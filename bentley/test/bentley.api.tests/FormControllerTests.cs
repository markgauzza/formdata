using bentley.Api.Controllers;
using bentley.Api.Models;
using bentley.Api.Repositories;
using bentley.Api.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace bentley.Api.Tests.Controllers
{
    public class FormsControllerTests
    {
        private readonly Mock<IFormDataRepository> _repoMock;
        private readonly Mock<ILogger<FormsController>> _loggerMock;
        private readonly Mock<IValidator<IFormValidatable>> _validatorMock;
        private readonly FormsController _controller;

        public FormsControllerTests()
        {
            _repoMock = new Mock<IFormDataRepository>();
            _loggerMock = new Mock<ILogger<FormsController>>();
            _validatorMock = new Mock<IValidator<IFormValidatable>>();

            _controller = new FormsController(
                _repoMock.Object,
                _loggerMock.Object,
                _validatorMock.Object);

            // Default: make the user authenticated so UserCanView / UserCanModify pass
            SetupAuthenticatedUser("user-123");
        }

        #region Helper methods

        private void SetupAuthenticatedUser(string userId, bool isAdmin = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("sub", userId)
            };

            if (isAdmin)
                claims.Add(new Claim("admin", "true"));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new CreateFormRequest(Guid.NewGuid(), "subject", "description", null, null, null, DateTime.UtcNow);

            _validatorMock
                .Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var createdResponse = new FormResponse(Guid.NewGuid(), "subject", "description", null, null, null, DateTime.UtcNow, null, "mbg", "abc");
       
            _repoMock
                .Setup(r => r.CreateFormRequestAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                    It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>()))
                .ReturnsAsync(createdResponse);   // ← FormResponse

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(createdResponse);
        }

        [Fact]
        public async Task Create_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateFormRequest(Guid.NewGuid(), "subject", "description", null, null, null, DateTime.UtcNow);

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Subject", "Subject is required")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        } 

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var form = new FormResponse(id, "Existing", "description", null, null, null, DateTime.UtcNow, null, "mbg", "abc");

            _repoMock
                .Setup(r => r.GetFormRequestByIdAsync(id))
                .ReturnsAsync(form);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(form);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetFormRequestByIdAsync(id))
                .ReturnsAsync((FormResponse?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region List

        [Fact]
        public async Task List_ReturnsOkWithResults()
        {
            // Arrange
            var query = new FormListQuery { Page = 1, PageSize = 10 };
            var forms = new List<FormData>
            {
                new FormData { Subject = "Item 1" },
                new FormData { Subject = "Item 2" }
            };

            var response = new FormDataList
            {
                PageNumber = 1,
                PageSize = 10,
                TotalRecords = 2,
                Results = forms
            };

            _repoMock
                .Setup(r => r.GetFormDataListAsync(1, 10, null))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.List(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ExistingForm_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateFormRequest(Guid.NewGuid(), "subhject", "description", null, null, null, DateTime.UtcNow);

            var form = new FormResponse(id, "Existing", "description", null, null, null, DateTime.UtcNow, null, "mbg", "abc");
            _repoMock
                .Setup(r => r.GetFormRequestByIdAsync(id))
                .ReturnsAsync(form);


            _validatorMock
                .Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var updatedForm = new FormResponse(id, "subject", "description", null, null, null, DateTime.UtcNow, DateTime.UtcNow, "mbg", "abc");
            _repoMock
                .Setup(r => r.UpdateFormRequestAsync(
                    id, request.Subject, request.Description,
                    false, request.DueDate, request.Priority, It.IsAny<string>()))
                .ReturnsAsync(updatedForm);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(updatedForm);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateFormRequest(Guid.NewGuid(), "subject", "description", null, null, null, DateTime.UtcNow); 

            _validatorMock
                .Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var response = new FormResponse(id, "subject", "description", null, null, null, DateTime.UtcNow, DateTime.UtcNow, "mbg", "abc");

            _repoMock
                .Setup(r => r.UpdateFormRequestAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateTime?>(),
                    It.IsAny<int?>(), It.IsAny<string>()))
               .ReturnsAsync((FormResponse?)null);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ExistingId_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var deletedForm = new FormData { Id = id, Subject = "A form" };

            _repoMock
                 .Setup(r => r.DeleteFormRequestAsync(id))
                 .ReturnsAsync(1);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                 .Setup(r => r.GetFormRequestByIdAsync(id))
                 .ReturnsAsync((FormResponse?)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Exception handling

        [Fact]
        public async Task GetById_RepositoryThrows_ReturnsProblem()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetFormRequestByIdAsync(id))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(500);
        }

        #endregion
    }
}