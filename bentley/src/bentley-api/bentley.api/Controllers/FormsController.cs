using bentley.api.Repositories;
using bentley.api.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bentley.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class FormsController(IFormDataRepository formDataRepository, ILogger<FormsController> logger, IValidator<IFormValidatable> validator) : ControllerBase
    {
        #region Public Methods

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormRequest request)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var formData = await formDataRepository.CreateFormRequestAsync(request.Subject,
                        request.Description, request.Critical ?? false,
                        request.DueDate, request.Priority, request.CreatedBy);

                return Ok(formData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new form request. {request}", request);
                return Problem("An error occurred while creating a new form request.");
            }
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");
                var result = await formDataRepository.GetFormRequestByIdAsync(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the form request with ID {FormRequestId}.", id);
                return Problem(string.Format("An error occurred while retrieving the form request with ID {0}.", id));
            }
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FormListQuery query)
        {
            var result = await formDataRepository.GetFormDataListAsync(query.Page, query.PageSize, query.SubjectFilter);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFormRequest request)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var formData = await formDataRepository.UpdateFormRequestAsync(id, request.Subject, request.Description, request.Critical.HasValue ? request.Critical.Value : false, request.DueDate, request.Priority, request.UpdatedBy);
                if (formData == null)
                    return NotFound();

                return Ok(formData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the form request with ID {FormRequestId}.", id);
                return Problem(string.Format("An error occurred while updating the form request with ID {0}.", id));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await formDataRepository.DeleteFormRequestAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting the form request with ID {FormRequestId}.", id);
                return Problem(string.Format("An error occurred while deleting the form request with ID {0}.", id));
            }
        }

        #endregion

        #region Private Methods        

        #endregion
    }
}
