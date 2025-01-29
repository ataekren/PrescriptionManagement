using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrescriptionService.Services;
using Shared.Models;
using SharedKernel.Models.Prescriptions;
using SharedKernel.Models.Prescriptions.DTOs;
using System.Security.Claims;

namespace PrescriptionService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PrescriptionController> _logger;

    public PrescriptionController(
        IPrescriptionService prescriptionService,
        INotificationService notificationService,
        ILogger<PrescriptionController> logger)
    {
        _prescriptionService = prescriptionService;
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<PrescriptionResponse>> CreatePrescription(
        [FromBody] CreatePrescriptionRequest request)
    {
        try
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (doctorId == 0)
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { message = "Invalid user identification" });
            }

            var prescription = await _prescriptionService.CreatePrescriptionAsync(doctorId, request);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("submit")]
    [Authorize(Roles = "Pharmacy")]
    public async Task<ActionResult<PrescriptionResponse>> SubmitPrescription(
        [FromBody] SubmitPrescriptionRequest request)
    {
        try
        {
            var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (pharmacyId == 0)
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { message = "Invalid user identification" });
            }

            var prescription = await _prescriptionService.SubmitPrescriptionAsync(pharmacyId, request);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting prescription");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrescriptionResponse>> GetPrescription(int id)
    {
        try
        {
            var prescription = await _prescriptionService.GetPrescriptionAsync(id);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving prescription {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<ActionResult<PagedResponse<PrescriptionResponse>>> GetPrescriptions(
        [FromQuery] PrescriptionStatus? status,
        [FromQuery] PaginationRequest pagination)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized();
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var prescriptions = await _prescriptionService.GetPrescriptionsAsync(
                role == "Doctor" ? userId : null,
                null,  // pharmacyId is not needed for Doctor/Patient roles
                status
            );
        
            var totalCount = prescriptions.Count();
            var items = prescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var response = new PagedResponse<PrescriptionResponse>(items, pagination.PageNumber, pagination.PageSize, totalCount);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prescriptions");
            return StatusCode(500, "An error occurred while getting prescriptions");
        }
    }

    //public endpoint that returns prescriptions that are not completed
    [HttpGet("not-completed")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<PrescriptionResponse>>> GetNotCompletedPrescriptions(
        [FromQuery] PaginationRequest pagination)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsAsync(status: PrescriptionStatus.PartiallySubmitted);
        
            var totalCount = prescriptions.Count();
            var items = prescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var response = new PagedResponse<PrescriptionResponse>(items, pagination.PageNumber, pagination.PageSize, totalCount);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prescriptions");
            return StatusCode(500, "An error occurred while getting prescriptions");
        }
    }

    [HttpPost("send-incomplete-notifications")]
    public async Task<IActionResult> SendIncompleteNotifications()
    {
        try
        {
            await _notificationService.SendIncompleteNotificationsAsync();
            return Ok(new { message = "Notifications sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending incomplete prescription notifications");
            return StatusCode(500, new { message = "Error sending notifications" });
        }
    }
} 