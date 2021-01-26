#pragma checksum "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ca49ba742682eb351c415a032880d2d3c3d7eeb9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Sessie_Evaluatie), @"mvc.1.0.view", @"/Views/Sessie/Evaluatie.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ca49ba742682eb351c415a032880d2d3c3d7eeb9", @"/Views/Sessie/Evaluatie.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e77d700a558efb6c6e1f873c8ab7518767964e7c", @"/Views/_ViewImports.cshtml")]
    public class Views_Sessie_Evaluatie : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Taalcafe.Models.DB.Sessie>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/Chart.js/dist/Chart.bundle.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
  
    ViewData["Title"] = "Sessie evaluatie";
    
    int angryCount = 0;
    int neutralCount = 0;
    int happyCount = 0;

    foreach (var sp in Model.SessiePartners)
    {
        if (sp.CijferTaalcoach == 1) {
            angryCount += 1;
        }
        else if (sp.CijferTaalcoach == 2){
            neutralCount += 1;
        }
        else if (sp.CijferTaalcoach == 3){
            happyCount += 1;
        }

        if (sp.CijferCursist == 1) {
            angryCount += 1;
        }
        else if (sp.CijferCursist == 2){
            neutralCount += 1;
        }
        else if (sp.CijferCursist == 3){
            happyCount += 1;
        }
    }
            

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ca49ba742682eb351c415a032880d2d3c3d7eeb94468", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
    <script>
        // Visit https://www.chartjs.org/ for documentation on the library used for plotting graphs.
        // See https://www.chartjs.org/docs/latest/axes/labelling.html for custom labeling (probably needed).
        var ctx = document.getElementById('EvaluationChart');
        var EvaluationChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Angry', 'Neutral', 'Happy'],
                datasets: [{
                    data: [");
#nullable restore
#line 45 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                      Write(angryCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(", ");
#nullable restore
#line 45 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                   Write(neutralCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(", ");
#nullable restore
#line 45 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                                  Write(happyCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"],
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            precision: 0,
                        }
                    }]
                },
                legend: {
                    display: false
                }
            }
        });
    </script>
");
            }
            );
            WriteLiteral("\r\n<div class=\"container-fluid pt-0\">\r\n    <div class=\"row ml-3 mb-3 justify-content-center\">\r\n        <h3><u>Dashboard evaluaties taalcafé sessie: ");
#nullable restore
#line 78 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                                Write(Model.Datum.Value.ToShortDateString());

#line default
#line hidden
#nullable disable
            WriteLiteral(" van ");
#nullable restore
#line 78 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                                                                           Write(Model.Datum.Value.ToString("HH:mm"));

#line default
#line hidden
#nullable disable
            WriteLiteral(" tot ");
#nullable restore
#line 78 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                                                                                                                    Write(Model.Datum.Value.Add(Model.Duur.Value).ToString("HH:mm"));

#line default
#line hidden
#nullable disable
            WriteLiteral(", thema: ");
#nullable restore
#line 78 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                                                                                                                                                                                       Write(Model.Thema.Naam);

#line default
#line hidden
#nullable disable
            WriteLiteral(@" </u></h3>
    </div>
    <div class=""row mt-5"">
        <div class=""col-4 pr-5"">
            <div class=""row ml-3"">
                <h3>Beoordeling gesprekken</h3>
            </div>
            <div class=""row ml-3"">
                <canvas id=""EvaluationChart"" max-width=""200"" height=""200""></canvas>
            </div>
        </div>
        <div class=""col-4 mt-1"">
            <div class=""row ml-5"">
                <h3>Sessie duo's</h3>
            </div>
            <div class=""row ml-5"">
                <ul class=""list-group w-75"">
                    <li class=""list-group-item"">
                        <div class=""container-fluid"">
                            <div class=""row"">
                                <div class=""col-6"">
                                    <b>Taalcoach</b>
                                </div>
                                <div class=""col-6"">
                                    <b>Nieuwe Nederlander</b>
                                </div>
          ");
            WriteLiteral("                  </div>\r\n                        </div>\r\n                    </li>\r\n");
#nullable restore
#line 107 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                     foreach (var item in Model.SessiePartners)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                        <li class=""list-group-item"">
                            <div class=""container-fluid"">
                                <div class=""row"">
                                    <div class=""col-6"">
                                        ");
#nullable restore
#line 113 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                   Write(item.Taalcoach.Naam);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </div>\r\n                                    <div class=\"col-6\">\r\n                                        ");
#nullable restore
#line 116 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                                   Write(item.Cursist.Naam);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </div>\r\n                                </div>\r\n                            </div>\r\n                        </li>\r\n");
#nullable restore
#line 121 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </ul>
            </div>
        </div>
        <div class=""col-4 mt-1"">
            <div class=""row mr-3"">
                <h3>Toelichting op beoordeling</h3>
            </div>
            <div class=""row mr-3"">
                <p><i>Selecteer een toelichting om naar de gebruiker te gaan.</i></p>
            </div>
            <div class=""row mr-3"">
                <ul class=""list-group"">
");
#nullable restore
#line 134 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                     foreach (var item in Model.SessiePartners)
                    {
                        if (item.FeedbackTaalcoach != null) {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <li class=\"list-group-item\">\r\n                                ");
#nullable restore
#line 138 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                           Write(Html.DisplayTextFor(modelItem => item.FeedbackTaalcoach));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </li>\r\n");
#nullable restore
#line 140 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                        }

                        if (item.FeedbackCursist != null) {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <li class=\"list-group-item\">\r\n                                ");
#nullable restore
#line 144 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                           Write(Html.DisplayTextFor(modelItem => item.FeedbackCursist));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </li>\r\n");
#nullable restore
#line 146 "D:\Minor\Project\Code\TaalcafeMVC\Taalcafe\Taalcafe\Views\Sessie\Evaluatie.cshtml"
                        }
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </ul>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    \r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Taalcafe.Models.DB.Sessie> Html { get; private set; }
    }
}
#pragma warning restore 1591
