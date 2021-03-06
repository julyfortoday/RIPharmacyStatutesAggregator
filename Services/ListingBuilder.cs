﻿using RIPharmStatutesAggregator.Core;
using RIPharmStatutesAggregator.Core.StatuteElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIPharmStatutesAggregator.Services
{
    public static class ListingBuilder
    {
        public static StatuteListing BuildListing(List<Page> pages)
        {
            ExtractPageElements(pages);

            var indexPages = GetIndexPages(pages);
            var chapters = new List<Chapter>();

            foreach (var indexPage in indexPages)
            {
                var chapter = BuildChapter(indexPage, pages);
                chapters.Add(chapter);
            }

            var listing = new StatuteListing();
            listing.Titles = BuildTitles(chapters, pages);
            return listing;
        }

        public static void ExtractPageElements(List<Page> pages)
        {
            foreach (var page in pages)
            {
                page.Elements = PageElementExtractor.Extract(page.Html, IsIndexPage(page));
            }
        }

        public static List<Page> GetIndexPages(List<Page> pages)
        {
            var indexPages = pages.Where(x => IsIndexPage(x));
            return indexPages.ToList();
        }

        public static bool IsIndexPage(Page page)
        {
            return page.Path.Contains("INDEX.HTM");
        }

        public static List<Page> GetLinkedPagesForIndex(Page indexPage, List<Page> pages)
        {
            var chapterName = Path.GetFileName(indexPage.Path).Replace("_INDEX.HTM", "");
            var subPages = pages.Where(x => x.Path.Contains(chapterName)).ToList();
            subPages.Remove(indexPage);
            return subPages;
        }

        public static List<Title> BuildTitles(List<Chapter> chapters, List<Page> pages)
        {
            var titles = new List<Title>();

            foreach (var chapter in chapters)
            {

                var firstSectionPage = pages.FirstOrDefault(x =>
                    (x.Elements.ChapterNumber == chapter.ChapterNumber) && !IsIndexPage(x));

                Title title;
                if (titles.Any(x => x.TitleNumber == firstSectionPage.Elements.TitleNumber))
                {
                    title = titles.FirstOrDefault(x => x.TitleNumber == firstSectionPage.Elements.TitleNumber);
                }
                else
                {
                    title = new Title()
                    {
                        TitleName = firstSectionPage.Elements.TitleName,
                        TitleNumber = firstSectionPage.Elements.TitleNumber
                    };
                    titles.Add(title);
                }

                title.Chapters.Add(chapter);
            }
            return titles;
        }

        public static Chapter BuildChapter(Page indexPage, List<Page> pages)
        {
            var subPages = GetLinkedPagesForIndex(indexPage, pages);
            var chapter = new Chapter() { };

            var first = subPages.FirstOrDefault();
            chapter.ChapterNumber = first.Elements.ChapterNumber;
            chapter.ChapterName = first.Elements.ChapterName;
            chapter.OriginalLink = indexPage.Address;

            foreach (var subPage in subPages)
            {
                var section = BuildSection(subPage);
                chapter.Sections.Add(section);
                if(HasArticle(subPage))
                {
                    var article = BuildArticle(subPage);
                    var matchedArticle = chapter.Articles.FirstOrDefault(x => x.ArticleNumber == article.ArticleNumber);

                    if (matchedArticle != null)
                    {
                        // pre-existing article, so use it instead of the new one
                        matchedArticle.Sections.Add(section);
                    }
                    else
                    {
                        // this is a new article, add it to chapter and use it
                        chapter.Articles.Add(article);
                        article.Sections.Add(section);
                    }
                }
            }

            return chapter;
        }

        public static bool HasArticle(Page page)
        {
            return !string.IsNullOrWhiteSpace(page.Elements.ArticleNumber);
        }

        public static Article BuildArticle(Page page)
        {
            var article = new Article();
            article.ArticleName = page.Elements.ArticleName;
            article.ArticleNumber = page.Elements.ArticleNumber;
            return article;
        }

        public static Section BuildSection(Page page)
        {
            var section = new Section();
            section.SectionName = page.Elements.SectionName;
            section.SectionNumber = page.Elements.SectionNumber;
            section.OriginalLink = page.Address;
            section.Lines = BuildLines(page.Elements.SectionContents);
            section.HistoryHeader = page.Elements.HistoryHeader;
            section.HistoryList = page.Elements.HistoryList;
            return section;
        }

        private static List<Line> BuildLines(List<Tuple<string, string>> sectionContents)
        {
            var lines = new List<Line>();
            foreach(var item in sectionContents)
            {
                var line = new Line();
                line.Identifier = item.Item1;
                line.Contents = item.Item2;
                lines.Add(line);
            }
            return lines;
        }
    }
}
