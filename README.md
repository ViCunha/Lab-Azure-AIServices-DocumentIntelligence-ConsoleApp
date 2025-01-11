
### Overview
---
In this exercise, you'll set up an Azure AI Document Intelligence resource in your Azure subscription. You'll use both the Azure AI Document Intelligence Studio and C# or Python to submit forms to that resource for analysis.
   
### Key Aspects
---
- Azure AI Serivces # Document Intelligence

### Environments
---
- Local
  - Microsoft Visual Studio 2022
    - .NET 9 (Standard Term Support)

- Microsoft Lab Sandbox
  - Microsfot Visual Studio Code
    - .NET 8 (Long Term Support)

- Azure Portal
  - Valid subscription

### Diagrams
---
![image](https://github.com/user-attachments/assets/4ba107eb-0fa9-4443-8339-ce26ccc7c443)


### Actions
---
Create the required resources
   - In a browser tab, open the Azure portal at https://portal.azure.com
  - In the Azure portal, select Create a resource
  - In the Search services and marketplace box, type Document Intelligence and then press Enter
  - In the Document intelligence page, select Create
  - In the Create Document intelligence page, under Project Details, select your Subscription and either select an existing Resource group or create a new one
  - Under Instance details, select a Region near your users
  - In the Name textbox, type a unique name for the resource
  - Select a Pricing tier and then select Review + create
  - If the validation tests pass, select Create. Azure deploys the new Azure AI Document Intelligence resource.

Using Read model
  - Open a new browser tab and go to the Azure AI Document Intelligence Studio at https://documentintelligence.ai.azure.com/studio
  - Under Document Analysis, select the Read tile.
    - If you are asked to sign into your account, use your Azure credentials.
    - If you are asked which Azure AI Document Intelligence resource to use, select the subscription and resource name you used when you created the Azure AI Document Intelligence resource.
  - In the list of documents on the left, select read-german.pdf
  - At the top-left, select Analyze options, then enable the Language check-box (under Optional detection) in the Analyze options pane and click on Save
  - At the top-left, select Run Analysis
  - When the analysis is complete, the text extracted from the image is shown on the right in the Content tab. Review this text and compare it to the text in the original image for accuracy.
  - Select the Result tab. This tab displays the extracted JSON code.
  - Scroll to the bottom of the JSON code in the Result tab. Notice that the read model has detected the language of each span indicated by locale. Most spans are in German (language code de) but you can find other language codes in the spans (e.g. English - language code en - in one of the last span)

Consume Azure AI Document Intelligence Processing
  - In the Azure portal, navigate to the Azure AI Document Intelligence resource
  - Under Resource Management, select Keys and Endpoint
  - Copy either KEY 1 or KEY 2 and the Endpoint values and store them for use in your application code
  - In  Microsoft Visual Studio, create a new ConsoleApp project
  - Add the NuGet package Azure.AI.FormRecognizer
  - In Program.cs, paste one of the codes below 
    - Possible code source: https://github.com/MicrosoftLearning/mslearn-ai-document-intelligence

```
using Azure;

using Azure.AI.FormRecognizer.DocumentAnalysis;

string endpoint = "<endpoint url>";

string key = "<API key>";

AzureKeyCredential credential = new AzureKeyCredential(key);

DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

Uri fileUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf");

AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-layout", fileUri);

AnalyzeResult result = operation.Value;

foreach (DocumentPage page in result.Pages)
{
    Console.WriteLine($"Document Page {page.PageNumber} has {page.Lines.Count} line(s), {page.Words.Count} word(s),");

    Console.WriteLine($"and {page.SelectionMarks.Count} selection mark(s).");

    for (int i = 0; i < page.Lines.Count; i++)
    {
        DocumentLine line = page.Lines[i];

        Console.WriteLine($"  Line {i} has content: '{line.Content}'.");

        Console.WriteLine($"    Its bounding polygon (points ordered clockwise):");

        for (int j = 0; j < line.BoundingPolygon.Count; j++)
        {
            Console.WriteLine($"      Point {j} => X: {line.BoundingPolygon[j].X}, Y: {line.BoundingPolygon[j].Y}");
        }
    }

    for (int i = 0; i < page.SelectionMarks.Count; i++)
    {
        DocumentSelectionMark selectionMark = page.SelectionMarks[i];

        Console.WriteLine($"  Selection Mark {i} is {selectionMark.State}.");

        Console.WriteLine($"    Its bounding polygon (points ordered clockwise):");

        for (int j = 0; j < selectionMark.BoundingPolygon.Count; j++)
        {
            Console.WriteLine($"      Point {j} => X: {selectionMark.BoundingPolygon[j].X}, Y: {selectionMark.BoundingPolygon[j].Y}");
        }
    }
}

Console.WriteLine("Paragraphs:");

foreach (DocumentParagraph paragraph in result.Paragraphs)
{
    Console.WriteLine($"  Paragraph content: {paragraph.Content}");

    if (paragraph.Role != null)
    {
        Console.WriteLine($"    Role: {paragraph.Role}");
    }
}

foreach (DocumentStyle style in result.Styles)
{
    bool isHandwritten = style.IsHandwritten.HasValue && style.IsHandwritten == true;

    if (isHandwritten && style.Confidence > 0.8)
    {
        Console.WriteLine($"Handwritten content found:");

        foreach (DocumentSpan span in style.Spans)
        {
            Console.WriteLine($"  Content: {result.Content.Substring(span.Index, span.Length)}");
        }
    }
}

Console.WriteLine("The following tables were extracted:");

for (int i = 0; i < result.Tables.Count; i++)
{
    DocumentTable table = result.Tables[i];
    Console.WriteLine($"  Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");

    foreach (DocumentTableCell cell in table.Cells)
    {
        Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) has kind '{cell.Kind}' and content: '{cell.Content}'.");
    }
}
```
```
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

// Store connection information
string endpoint = "<Endpoint URL>";
string apiKey = "<API Key>";

Uri fileUri = new Uri("https://github.com/MicrosoftLearning/mslearn-ai-document-intelligence/blob/main/Labfiles/01-prebuild-models/sample-invoice/sample-invoice.pdf?raw=true");

Console.WriteLine("\nConnecting to Forms Recognizer at: {0}", endpoint);
Console.WriteLine("Analyzing invoice at: {0}\n", fileUri.ToString());

// Create the client
var cred = new AzureKeyCredential(apiKey);
var client = new DocumentAnalysisClient(new Uri(endpoint), cred);

// Analyze the invoice
AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", fileUri);

// Display invoice information to the user
AnalyzeResult result = operation.Value;

foreach (AnalyzedDocument invoice in result.Documents)
{
    if (invoice.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField))
    {
        if (vendorNameField.FieldType == DocumentFieldType.String)
        {
            string vendorName = vendorNameField.Value.AsString();
            Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}.");
        }
    }

    if (invoice.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField))
    {
        if (customerNameField.FieldType == DocumentFieldType.String)
        {
            string customerName = customerNameField.Value.AsString();
            Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}.");
        }
    }

    if (invoice.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField))
    {
        if (invoiceTotalField.FieldType == DocumentFieldType.Currency)
        {
            CurrencyValue invoiceTotal = invoiceTotalField.Value.AsCurrency();
            Console.WriteLine($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}.");
        }
    }
}
Console.WriteLine("\nAnalysis complete.\n");
```


- Run the code

### Media
---
![image](https://github.com/user-attachments/assets/340d5a39-4323-4fa8-bbd5-2b37d75f6f18)
---
![image](https://github.com/user-attachments/assets/f3752412-d29e-48e0-a984-910b61e638fe)
---
![image](https://github.com/user-attachments/assets/4af6b5b8-97b7-4a3b-9a2c-50ad36e513dd)
---
![image](https://github.com/user-attachments/assets/4b73d655-79d5-4a18-9274-d20c94ee561e)
---
![image](https://github.com/user-attachments/assets/0301fd0f-1833-4143-a1b8-2fd8231e4199)
---
![image](https://github.com/user-attachments/assets/aa27491f-5949-4b74-a567-895a26a66f4a)
---
![image](https://github.com/user-attachments/assets/3f60b070-88d7-48d2-ae6b-d7b37d05837e)
---
![image](https://github.com/user-attachments/assets/d76b9f19-e979-449f-9b22-0d278d2ff8e3)
---
![image](https://github.com/user-attachments/assets/1ff53f98-cbe5-49c2-89cd-91ed46368fe8)
---
![image](https://github.com/user-attachments/assets/15a99ea3-4437-42d7-967c-856384cf20c3)
---
![image](https://github.com/user-attachments/assets/7b4cac25-4e15-4e28-a1b6-3f09d27d738d)
---
![image](https://github.com/user-attachments/assets/add1e8eb-08a8-46aa-bc53-462cae300ab3)






### References
---
- [Plan Azure AI Document Intelligence resources - Training | Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/plan-form-recognizer-solution/3-resources?pivots=csharp)
- [Quickstart: Document Intelligence client libraries - Azure AI services | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api?view=doc-intel-3.0.0&preserve-view=true&pivots=programming-language-csharp)
