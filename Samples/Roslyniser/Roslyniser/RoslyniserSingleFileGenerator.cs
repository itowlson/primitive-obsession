using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Roslyniser
{
    [ComVisible(true)]
    [Guid("E9616DE9-0BBD-4F0E-AF73-8EAAA555F429")]
    [CodeGeneratorRegistration(typeof(RoslyniserSingleFileGenerator), "RoslyniserSingleFileGenerator", "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}" /*vsContextGuids.vsContextGuidVCSProject*/, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(RoslyniserSingleFileGenerator))]
    public class RoslyniserSingleFileGenerator : IVsSingleFileGenerator, IObjectWithSite
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".g.cs";
            return VSConstants.S_OK;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            string result = Transform(Path.GetFileName(wszInputFilePath), bstrInputFileContents);

            byte[] buf = Encoding.UTF8.GetBytes(result);

            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(buf.Length);
            Marshal.Copy(buf, 0, rgbOutputFileContents[0], buf.Length);
            pcbOutput = (uint)(buf.Length);

            return VSConstants.S_OK;
        }

        private object _site;

        public void SetSite(object pUnkSite)
        {
            _site = pUnkSite;
        }

        public void GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (_site == null)
            {
                throw new COMException("object is not sited", VSConstants.E_FAIL);
            }

            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(_site);
            IntPtr intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

            if (intPointer == IntPtr.Zero)
            {
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
            }

            ppvSite = intPointer;
        }

        private string Transform(string fileName, string source)
        {
            var sp = new ServiceProvider(_site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            var vsproj = ((EnvDTE.ProjectItem)(sp.GetService(typeof(EnvDTE.ProjectItem)))).ContainingProject;
            var cm = (IComponentModel)(Package.GetGlobalService(typeof(SComponentModel)));

            var workspace = cm.GetService<VisualStudioWorkspace>();

            var solution = workspace.CurrentSolution;
            var project = solution.Projects.FirstOrDefault(p => p.FilePath == vsproj.FileName);

            var syntaxTrees = Enumerable.Empty<SyntaxTree>();

            if (project != null)
            {
                var c = project.GetCompilationAsync().Result;
                syntaxTrees = c.SyntaxTrees;
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var compilation = CSharpCompilation.Create("temp", syntaxTrees.Concat(new[] { syntaxTree }));

            var rewriter = new PrimitiveObsessorRewriter(fileName, compilation.GetSemanticModel(syntaxTree, true));

            var result = rewriter.Visit(syntaxTree.GetRoot());

            return result.ToFullString();
        }

        private class PrimitiveObsessorRewriter : CSharpSyntaxRewriter
        {
            private readonly string _sourceFile;
            private readonly SemanticModel _semanticModel;

            private readonly List<FieldDeclarationSyntax> _fieldsToAdd = new List<FieldDeclarationSyntax>();

            public PrimitiveObsessorRewriter(string sourceFile, SemanticModel semanticModel)
            {
                _sourceFile = sourceFile;
                _semanticModel = semanticModel;
            }

            public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
            {
                var wrapAttribute = node.AttributeLists.SelectMany(al => al.Attributes).SingleOrDefault(a => ((IdentifierNameSyntax)(a.Name)).Identifier.Text == "WrapsPrimitive");

                if (wrapAttribute != null)
                {
                    bool comparable = wrapAttribute.ArgumentList.Arguments.Any(a => a.NameEquals != null && a.NameEquals.Name.Identifier.Text == "Comparable" && a.Expression.ToString() == "true");

                    var fields = node.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

                    if (fields.Count != 1)
                    {
                        return node;
                    }

                    var field = fields.Single().Declaration;
                    var fieldId = field.Variables.Single().Identifier;

                    var str = node;

                    var list = node.AttributeLists.Single(al => al.Attributes.Contains(wrapAttribute));
                    if (list.Attributes.Count == 1)
                    {
                        str = str.RemoveNode(list, SyntaxRemoveOptions.KeepEndOfLine | SyntaxRemoveOptions.KeepExteriorTrivia);
                    }
                    else
                    {
                        str = str.RemoveNode(wrapAttribute, SyntaxRemoveOptions.KeepEndOfLine | SyntaxRemoveOptions.KeepExteriorTrivia);
                    }

                    str = (StructDeclarationSyntax)(base.VisitStructDeclaration(str));

                    str = str.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"System.IEquatable<{str.Identifier.Text}>")));

                    var contentFormat = @"
public {0}({2} raw)
{{
    {1} = raw;
}}

public bool Equals({0} other)
{{
    return other.{1} == {1};
}}

public override bool Equals(object other)
{{
    if (other is {0})
    {{
        return Equals(({0})other);
    }}
    return false;
}}

public override int GetHashCode()
{{
    return {1}.GetHashCode();  // TODO: handle where primitive is nullable
}}

public static bool operator ==({0} first, {0} second)
{{
    return first.Equals(second);
}}

public static bool operator !=({0} first, {0} second)
{{
    return !(first == second);
}}
";

                    if (comparable)
                    {
                        str = str.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"System.IComparable<{str.Identifier.Text}>")));

                        contentFormat += @"
public int CompareTo({0} other)
{{
    return {1}.CompareTo(other.{1});
}}

public static bool operator <({0} first, {0} second)
{{
    return first.CompareTo(second) < 0;
}}

public static bool operator <=({0} first, {0} second)
{{
    return first.CompareTo(second) <= 0;
}}

public static bool operator >({0} first, {0} second)
{{
    return first.CompareTo(second) > 0;
}}

public static bool operator >=({0} first, {0} second)
{{
    return first.CompareTo(second) >= 0;
}}
";
                    }

                    var dummyStruct = @"
struct {0}
{{" + contentFormat + @"
}}";
                    
                    var text = String.Format(dummyStruct, str.Identifier.Text, fieldId.Text, field.Type);

                    var members = SyntaxFactory.ParseSyntaxTree(text)
                                            .GetRoot()
                                            .DescendantNodes()
                                            .OfType<StructDeclarationSyntax>()
                                            .Single()
                                            .Members
                                            .ToArray();

                    str = str.AddMembers(members);

                    str = str.NormalizeWhitespace();

                    return str;
                }

                return node;
            }
        }
    }
}
