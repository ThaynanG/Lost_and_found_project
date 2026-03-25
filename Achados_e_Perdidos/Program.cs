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
client.DefaultRequestHeaders.Add("ApplicationAccessKey", "V2-8RRaL-xVII8-Ooq7Q-ZOmhI-h9dra-Wfceg-TxLVy-XkS1O");

// SUA URL
var url = "https://www.appsheet.com/api/v2/apps/c1da97fa-952d-4a5b-b534-f5aac6054f87/tables/Pagina1/Action";

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




Console.WriteLine(result);
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

    // TÍTULO
    var titulo = new Paragraph($"RELATÓRIO - {grupo.Key}");
    titulo.SetFontSize(18);
    document.Add(titulo);

    document.Add(new Paragraph(" "));

    // TABELA (GRID DE IMAGENS)
    var table = new Table(3); // 3 imagens por linha





    foreach (var item in grupo)
    {
        try
        {
            var urlImagem = $"https://www.appsheet.com/template/gettablefileurl?appName=AchadosePerdidosRedHouse-716952994-26-03-23&tableName=Pagina1&fileName={item.Foto}";

            var bytes = await client.GetByteArrayAsync(item.FotoUrl);

            var imageData = iText.IO.Image.ImageDataFactory.Create(bytes);
            var image = new Image(imageData);

            image.SetWidth(150);
            image.SetHeight(150);
            image.SetAutoScale(true);

            var cell = new Cell().Add(image);
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            table.AddCell(cell);
        }
        catch
        {
            var cell = new Cell().Add(new Paragraph("Sem imagem"));
            cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
            table.AddCell(cell);
        }
    }

    document.Add(table);
    document.Close();








}

// CLASSE DOS DADOS
class Item
{
    [JsonProperty("FOTO_URL")]
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

