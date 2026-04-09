using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;

var client = new HttpClient();

// API KEY
client.DefaultRequestHeaders.Add("ApplicationAccessKey", "API");

// SUA URL
var url = "URL";

var body = @"{
    ""Action"": ""Find"",
    ""Properties"": {},
    ""Rows"": []
}";

var response = await client.PostAsync(
    url,
    new StringContent(body, Encoding.UTF8, "application/json")
);

var result = await response.Content.ReadAsStringAsync();

Console.WriteLine("Status: " + response.StatusCode);
Console.WriteLine(result);

// 🔥 VALIDAÇÃO IMPORTANTE
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine("ERRO NA API - verifique a API Key ou permissões.");
    return;
}

// CONVERTER JSON EM LISTA
var itens = JsonConvert.DeserializeObject<List<Item>>(result) ?? new List<Item>();

// FILTRAR ITENS NÃO DEVOLVIDOS
var itensPerdidos = itens
    .Where(x => string.IsNullOrWhiteSpace(x.DataEntrega))
    .ToList();

// AGRUPAR POR CATEGORIA
var grupos = itensPerdidos
    .GroupBy(x => x.Categoria)
    .ToList();




Console.WriteLine($"Total itens: {itens.Count}");
Console.WriteLine($"Não entregues: {itensPerdidos.Count}");
Console.WriteLine("Status: " + response.StatusCode);
Console.WriteLine(result);

foreach (var grupo in grupos)
{
    Console.WriteLine($"Categoria: {grupo.Key} - Quantidade: {grupo.Count()}");
}


// GERAR PDF POR CATEGORIA
foreach (var grupo in grupos)
{
    var caminho = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        $"Relatorio_{grupo.Key}.pdf"
    );

    var writer = new PdfWriter(caminho);
    var pdf = new PdfDocument(writer);
    var document = new Document(pdf);

    var titulo = new Paragraph($"Lost & Found - {grupo.Key}");
    titulo.SetFontSize(18);
    document.Add(titulo);

    document.Add(new Paragraph(" "));

    var table = new Table(3);

    foreach (var item in grupo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.FotoUrl) || !item.FotoUrl.StartsWith("{"))
                throw new Exception("Formato inválido");

            var fotoObj = JsonConvert.DeserializeObject<FotoObjeto>(item.FotoUrl);

            if (fotoObj == null || string.IsNullOrWhiteSpace(fotoObj.Url))
                throw new Exception("URL inválida");

            var responseImg = await client.GetAsync(fotoObj.Url);

            if (!responseImg.IsSuccessStatusCode)
                throw new Exception("Erro ao baixar");

            var bytes = await responseImg.Content.ReadAsByteArrayAsync();

            var imageData = iText.IO.Image.ImageDataFactory.Create(bytes);
            var image = new Image(imageData);

            image.SetWidth(150);
            image.SetHeight(150);

            var cell = new Cell().Add(image);
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            table.AddCell(cell);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro imagem: {ex.Message}");

            var cell = new Cell().Add(new Paragraph("Imagem inválida"));
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            table.AddCell(cell);
        }
    }

    // ✅ AGORA SIM (fora do loop)
    document.Add(table);
    document.Close();

    Console.WriteLine($"PDF gerado: {caminho}");
}

  


// CLASSE DOS DADOS
class Item

{
    [JsonProperty("FOTO URL")]
    public string FotoUrl { get; set; }

    [JsonProperty("NOME DO ITEM")]
    public string NomeItem { get; set; }        

    [JsonProperty("CATEGORIA")]
    public string Categoria { get; set; }

    [JsonProperty("DATA QUE FOI RETIRADO")]
    public string DataEntrega { get; set; }

    [JsonProperty("DATA QUE FOI PERDIDO")]
    public string DataPerda { get; set; }

    [JsonProperty("FOTO")]
    public string Foto { get; set; }

    [JsonProperty("TAMANHO")]
    public string Tamanho { get; set; }

    [JsonProperty("ID")]
    public string Id { get; set; }

    [JsonProperty("PESSOA QUE RETIROU")]
    public string PessoaRetirou { get; set; }
}

class FotoObjeto
{
    public string Url { get; set; }
    public string LinkText { get; set; }
}
