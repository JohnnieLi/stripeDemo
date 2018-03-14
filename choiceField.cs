using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Telerik.WebControls;
using Telerik.Charting;
using Telerik.WebControls.Dialogs;


namespace CommonControls.Survey
{
[Guid("ce0403df-d85c-4ba0-9b57-f944fb70ded4")]
public class SurveyChart : Microsoft.SharePoint.WebPartPages.WebPart
{
List<Response> response;
String surveyQuestion;
RadChart chart;


public SurveyChart()
{
this.ExportMode = WebPartExportMode.All;
}

protected override void RenderContents(HtmlTextWriter htmlTextWriter)
{
htmlTextWriter.Write("<TABLE><TR><TD>");
this.chart.RenderControl(htmlTextWriter);
htmlTextWriter.Write("</TABLE></TD></TR>");
}

protected override void CreateChildControls()
{
string[] colors = { "#FFDA43", "#2F89C2", "#F267CD", "#79E364", "#EB5D2D", "#42AE18", "#FF9C40", "#B99A72", "#9BC0FF", "#98CC88", "#C3B600" };
chart = new RadChart();
chart.UseSession = false;
chart.TempImagesFolder = "~/_layouts/temp";
chart.RadControlsDir = "~/_layouts/radcontrols/RadControls/";

ChartSeries chartSeries = new ChartSeries();
chart.AddChartSeries(chartSeries);
ChartTitle title = new ChartTitle();


ChartSeries series = chart.GetChartSeries(0);
series.Name = surveyQuestion;

chart.Legend.HAlignment = ChartHAlignment.Center;
chart.Legend.VAlignment = ChartVAlignment.Bottom;
chart.Legend.LegendStyle = LegendItemsPositionType.Row;


response = GetSurveyResponse("http://server/sites/techresources/", "http://server/sites/techresources/Lists/Survey2");
title.Text = surveyQuestion;
title.TextFont = new Font("Verdana", 7);
chart.Chart.ChartTitle = title;

series.Clear();
series.DiameterScale = 0.40;
series.ShowLabelConnectors = false; 
series.ShowLabels = true;
series.LabelAppearance.Distance = 7; 
series.LabelAppearance.TextFont = new Font("Verdana", 7);
series.NotOverlapLabels = true;
series.Type = ChartSeriesType.Pie;
ColorConverter cc = new ColorConverter();
int i = 0;
foreach (Response res in response)
{
ChartSeriesItem seriesItem = new ChartSeriesItem();
seriesItem.YValue = res.totalResponse;
seriesItem.ItemMap.ToolTip = res.choiceName;
seriesItem.Label = res.totalResponse.ToString();
seriesItem.Appearance.MainColor = (Color) cc.ConvertFromString(colors[i++]);
seriesItem.Appearance.BorderColor = Color.DimGray;
seriesItem.Appearance.FillStyle = FillStyle.Solid;
seriesItem.Name = res.choiceName;
series.Items.Add(seriesItem);
} 
chart.Skin = "LightGreen"; 
this.Controls.Add(chart);
}




public List<Response> GetSurveyResponse(string siteUrl, string surveyName)
{

List<Response> responseList;
string questionName = "";
using (SPSite oSPSite = new SPSite(siteUrl))
{
using (SPWeb oSPWeb = oSPSite.OpenWeb())
{
SPList surveyList = oSPWeb.GetList(surveyName);
if (surveyList.BaseType != SPBaseType.Survey)
throw new Exception("List is not of survey type");
responseList = new List<Response>();
foreach (SPField field in surveyList.Fields)
{
if (field.Type == SPFieldType.Choice)
{
SPFieldChoice choiceField;
choiceField = (SPFieldChoice)field;
questionName = field.StaticName;
surveyQuestion = field.Title;
foreach (string choiceName in choiceField.Choices)
{
Response resField = new Response();
resField.choiceName = choiceName;
responseList.Add(resField);
}
}
}
foreach (Response res in responseList)
{
SPQuery responseQuery = new SPQuery();
responseQuery.Query = @"<Where>
<Eq><FieldRef Name=" + questionName + " /><Value Type=\"Choice\">" + res.choiceName + @"</Value></Eq>
</Where>";
res.totalResponse = surveyList.GetItems(responseQuery).Count;
}
}
}
return responseList;
}
}
}