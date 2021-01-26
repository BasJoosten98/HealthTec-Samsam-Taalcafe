#pragma checksum "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "44fb7b4dee8d41e5c83dad538de07c6fab4c07be"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Sessie_EvaluatieOverview), @"mvc.1.0.view", @"/Views/Sessie/EvaluatieOverview.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\_ViewImports.cshtml"
using Taalcafe;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\_ViewImports.cshtml"
using Taalcafe.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"44fb7b4dee8d41e5c83dad538de07c6fab4c07be", @"/Views/Sessie/EvaluatieOverview.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e77d700a558efb6c6e1f873c8ab7518767964e7c", @"/Views/_ViewImports.cshtml")]
    public class Views_Sessie_EvaluatieOverview : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<Taalcafe.Models.DB.Sessie>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Evaluatie", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-success p-2"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
  
    ViewData["Title"] = "Evaluatie overzicht";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""row justify-content-center""  style=""margin-top:5%"">
    <div class=""col-md-auto w-75"">
        <div class=""d-inline-block align-middle col-md-12 pl-0"">
            <h3 class=""d-inline-block align-middle float-left"">Evaluaties Taalcafé sessies</h3>
        </div>
        <div style=""height: 310px;overflow: auto;"" class=""table-responsive-md"">
            <table class=""table-borderless w-100 text-center justify-content-center"">
                <thead>
                    <tr class=""d-flex"">
                        <th class=""col-md-2 align-middle table-secondary border border-white align-middle"">
                            <label class=""mt-2"">Datum</label>
                        </th>
                        <th class=""col-md-2 align-middle table-secondary border border-white"">
                            <label class=""mt-2"">Tijd</label>
                        </th>
                        <th class=""col-md-2 align-middle table-secondary border border-white"">
                    ");
            WriteLiteral(@"        <label class=""mt-2""> Duur</label>
                        </th>
                        <th class=""col-md-2 align-middle table-secondary border border-white"">
                            <label class=""mt-2"">Thema</label>
                        </th>
                        <th class=""col-md-2 align-middle table-secondary border border-white"">
                            <label class=""mt-2"">Evaluaties</label>
                        </th>
                    </tr>
                </thead>
                <tbody>
");
#nullable restore
#line 33 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                     foreach (var item in Model)
                    {
                        if (item.Datum.Value.Add(item.Duur.Value) <= DateTime.Now)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr class=\"d-flex\">\r\n                        <td class=\"col-md-2 align-middle table-active border border-white\">\r\n                            <label class=\"mt-3\">");
#nullable restore
#line 39 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                           Write(item.Datum.Value.ToShortDateString());

#line default
#line hidden
#nullable disable
            WriteLiteral("</label>\r\n                        </td>\r\n                        <td class=\"col-md-2 align-middle table-active border border-white\">\r\n                            <label class=\"mt-3\">");
#nullable restore
#line 42 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                           Write(item.Datum.Value.ToString("HH:mm"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</label>\r\n                        </td>\r\n                        <td class=\"col-md-2 align-middle table-active border border-white\">\r\n                            <label class=\"mt-3\"> ");
#nullable restore
#line 45 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                            Write(item.Duur.Value.ToString(@"h\:mm"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</label>\r\n                        </td>\r\n                        <td class=\"col-md-2 align-middle table-active border border-white\">\r\n                            <label class=\"mt-3\"> ");
#nullable restore
#line 48 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                            Write(Html.DisplayFor(modelItem => item.Thema.Naam));

#line default
#line hidden
#nullable disable
            WriteLiteral("</label>\r\n                        </td>\r\n                        <td class=\"col-md-2 align-middle table-active border border-white\">\r\n                            <label class=\"mt-3\"> ");
#nullable restore
#line 51 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                            Write(item.Duur.Value.ToString(@"h\:mm"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</label>\r\n                        </td>\r\n                        <td class=\"col-md-2\">\r\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "44fb7b4dee8d41e5c83dad538de07c6fab4c07be8592", async() => {
                WriteLiteral("Evaluaties inzien");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 54 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                                                                                    WriteLiteral(item.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 57 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\EvaluatieOverview.cshtml"
                        }
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Taalcafe.Models.DB.Sessie>> Html { get; private set; }
    }
}
#pragma warning restore 1591
