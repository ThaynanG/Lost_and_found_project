# Achados_e_Perdidos
# 📦 Achados e Perdidos - Relatório em PDF
# 📦 Achados e Perdidos - Relatório em PDF

Aplicação em C# que consulta dados do AppSheet e gera relatórios em PDF com imagens dos itens perdidos.

---

## 🚀 Funcionalidades

* Consulta de dados via API do AppSheet
* Filtro de itens não entregues
* Agrupamento por categoria
* Download automático das imagens
* Geração de PDFs organizados por categoria

---

## 🛠️ Tecnologias

* C# (.NET)
* Newtonsoft.Json
* HttpClient
* iText7
* AppSheet API

---

## ⚙️ Configuração

Defina sua API Key:

```csharp
client.DefaultRequestHeaders.Add("ApplicationAccessKey", "SUA_API_KEY");
```

Configure a URL do AppSheet:

```csharp
var url = "Sua URL";
```

---

## 📸 Formato da imagem

O campo `FOTO URL` deve conter:

```json
{
  "Url": "https://drive.google.com/uc?export=download&id=ID_DA_IMAGEM"
}
```

---

## 📄 Saída

Os relatórios são gerados automaticamente na área de trabalho:

```
Relatorio_Categoria.pdf
```

Cada PDF contém imagens organizadas em grade por categoria.

---

## 👨‍💻 Autor

Thaynan Guilhen Carvalho
