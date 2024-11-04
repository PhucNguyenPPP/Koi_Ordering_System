using Common.DTO.General;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

[Route("odata")]
[ApiController]
public class PolicyController : ODataController
{
    private readonly IPolicyService _policyService;

    public PolicyController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpGet("all-policies")]
    [EnableQuery]
    [Authorize(Roles = "KoiFarmManager")]
    public async Task<IActionResult> GetAllPolicies()
    {
        var policies = await _policyService.GetAllPoliciesAsync();
        if (policies == null || !policies.Any())
        {
            return NotFound(new ResponseDTO("No policies found", 404, false, null));
        }
        return Ok(policies);
    }

    [HttpGet("{policyId}")]
    [Authorize(Roles = "KoiFarmManager")]
    public async Task<IActionResult> GetPolicyById(Guid policyId)
    {
        var policy = await _policyService.GetPolicyByIdAsync(policyId);
        if (policy == null)
        {
            return NotFound(new ResponseDTO("Policy not found", 404, false, null));
        }
        return Ok(new ResponseDTO("Policy retrieved successfully", 200, true, policy));
    }

    [HttpPost("policy")]
    [Authorize(Roles = "KoiFarmManager")]
    public async Task<IActionResult> AddPolicy([FromBody] CreatePolicyRequest createPolicyRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResponseDTO("Invalid input", 400, false, ModelState));
        }

        var result = await _policyService.AddPolicyAsync(createPolicyRequest);
        if (result)
        {
            return Ok(new ResponseDTO("Policy added successfully", 200, true, null));
        }
        return BadRequest(new ResponseDTO("Failed to add policy", 400, false, null));
    }

    [HttpPut("policy")]
    [Authorize(Roles = "KoiFarmManager")]
    public async Task<IActionResult> UpdatePolicy([FromBody] PolicyDTO policyDTO)
    {
        var result = await _policyService.UpdatePolicyAsync(policyDTO);
        if (result)
        {
            return Ok(new ResponseDTO("Policy updated successfully", 200, true, null));
        }
        return BadRequest(new ResponseDTO("Failed to update policy", 400, false, null));
    }

    [HttpDelete("delete/{policyId}")]
    [Authorize(Roles = "KoiFarmManager")]
    public async Task<IActionResult> DeletePolicy(Guid policyId)
    {
        var result = await _policyService.DeletePolicyAsync(policyId);
        if (result)
        {
            return Ok(new ResponseDTO("Policy deleted successfully", 200, true, null));
        }
        return NotFound(new ResponseDTO("Policy not found", 404, false, null));
    }
    [HttpGet("policies-of-farm")]
    [EnableQuery]
    [Authorize(Roles = "KoiFarmManager,Customer")]
    public async Task<IActionResult> GetPolicyByFarm(Guid farmId)
    {
        var policies = await _policyService.GetPolicyByFarm(farmId);
        if (policies == null)
        {
            return NotFound(new ResponseDTO("Policy not found", 404, false, null));
        }
        return Ok(policies);
    }
}
