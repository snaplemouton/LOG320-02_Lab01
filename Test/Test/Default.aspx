<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Test._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Compression
    </h2>
    <input type="file" id="fileDo" />
    <button id="btnDo" onclick="doThis()">Compresser ce fichier</button>
    <br />
    <hr />
    <h2>
        Décompression
    </h2>
    <input type="file" id="fileDedo" />
    <button id="btnDedo">Décompresser ce fichier</button>
</asp:Content>
