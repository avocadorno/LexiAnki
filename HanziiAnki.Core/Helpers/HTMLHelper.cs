﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace HanziiAnki.Core.Helpers;

public class HTMLHelper
{
    public static string GetWrapped(string text, string tag) => $"<{tag}>{text}</{tag}>";
    public static string GetBold(string text) => GetWrapped(text, "b");
    public static string GetItalic(string text) => GetWrapped(text, "i");
    public static string GetUnderline(string text) => GetWrapped(text, "u");
    public static string GetItemList(string text) => GetWrapped(text, "li");

    public static string GetOrderedList(List<string> items)
    {
        var result = string.Join("\n", items.Select(item => GetItemList(item)));
        return GetWrapped(result, "ol");
    }

    public static string GetUnorderedList(List<string> items)
    {
        var result = string.Join("\n", items.Select(item => GetItemList(item)));
        return GetWrapped(result, "ul");
    }

    public static string GetBeautified(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument(html);

        var sw = new StringWriter();
        document.ToHtml(sw, new PrettyMarkupFormatter
        {
            Indentation = "  ", // Two spaces for indentation
            NewLine = "\n"      // New line character
        });

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(sw.ToString());
        return string.Join("\n", doc.QuerySelector("body").InnerHtml.Split("\n").Select(line => Unindent(line)).Where(line => line != "\n" && line != ""));
    }

    private static string Unindent(string text, int maxSpacesToTrim = 4)
    {
        var spaceCount = 0;

        for (var i = 0; i < text.Length && spaceCount < maxSpacesToTrim; i++)
        {
            if (text[i] == ' ')
            {
                spaceCount++;
            }
            else
            {
                break;
            }
        }

        return text.Substring(spaceCount);
    }
}
