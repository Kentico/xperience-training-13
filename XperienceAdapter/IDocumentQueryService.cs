using System;
using System.Collections.Generic;
using System.Text;

using CMS.DocumentEngine;
using Abstractions;

namespace XperienceAdapter
{
    public interface IDocumentService : IService
    {
        /// <summary>
        /// Wrapper around Kentico's DocumentQuery. 
        /// This query filters data based on active culture and handles the preview mode.
        /// </summary>
        /// <typeparam name="TDocument">Type of the generated page</typeparam>
        /// <param name="nodeGuid">NodeGuid of the page</param>
        /// <returns>DocumentQuery to a document identified by its nodeGuid</returns>
        DocumentQuery<TDocument> GetDocument<TDocument>(Guid nodeGuid) where TDocument : TreeNode, new();

        /// <summary>
        /// Wrapper around Kentico's DocumentQuery. 
        /// This query filters data based on active culture and handles the preview mode.
        /// </summary>
        /// <typeparam name="TDocument">Type of the generated page</typeparam>
        /// <param name="pageAlias">NodeAlias of the page</param>
        /// <returns>DocumentQuery to a document identified by its nodeGuid</returns>
        DocumentQuery<TDocument> GetDocument<TDocument>(string pageAlias) where TDocument : TreeNode, new();

        /// <summary>
        /// Wrapper around Kentico's DocumentQuery. 
        /// This query filters data based on active culture and handles the preview mode.
        /// </summary>
        /// <typeparam name="TDocument">Type of the generated page</typeparam>
        /// <returns>DocumentQuery documents</returns>
        DocumentQuery<TDocument> GetDocuments<TDocument>() where TDocument : TreeNode, new();
    }
}
