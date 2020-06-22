using System;
using Xunit;
using Moq;

using CMS.DocumentEngine;
using XperienceAdapter.Dtos;

namespace XperienceAdapter.Tests
{
    public class DocumentRepositoryTests
    {
        private Func<DocumentQuery<TreeNode>, DocumentQuery<TreeNode>> _zeroResultFilter = (query) =>
            query
                .WhereEquals("DocumentNodeID", 0)
                .Columns("DocumentGUID", "DocumentID", "DocumentName");

        private Func<TreeNode, TestDto> _selector = (document) => new TestDto
        {
            Guid = document.DocumentGUID,
            Id = document.DocumentID,
            Name = document.DocumentName,
            NodeId = document.NodeID
        };

        [Fact]
        public void GetDtos_DoesNotThrowWithEmptyQueryResults()
        {
            //var queryServiceMock = new Mock<IDocumentQueryService>();
            //var query = DocumentHelper.GetDocuments<TreeNode>();
            //queryServiceMock.Setup(service => service.GetDocuments<TreeNode>()).Returns(query);
            //var repository = new DocumentRepository(queryServiceMock.Object);

            //var result = repository.GetDtos(_zeroResultFilter, _selector);

            //Assert.NotNull(result);
        }
    }
}
