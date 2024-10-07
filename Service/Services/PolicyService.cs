using AutoMapper;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

public class PolicyService : IPolicyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PolicyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PolicyDTO>> GetAllPoliciesAsync()
    {
        // Use the UnitOfWork to get the Policy repository
        var policies = await _unitOfWork.Policy.GetAll().ToListAsync();

        // Map the entities to DTOs
        return _mapper.Map<List<PolicyDTO>>(policies);
    }

    public async Task<PolicyDTO> GetPolicyByIdAsync(Guid policyId)
    {
        // Use UnitOfWork to retrieve a single policy by ID
        var policy = await _unitOfWork.Policy.GetByCondition(p => p.PolicyId == policyId);
        if (policy == null)
        {
            return null;  // Could throw a KeyNotFoundException if you prefer
        }

        // Map the entity to DTO
        return _mapper.Map<PolicyDTO>(policy);
    }

    public async Task<bool> AddPolicyAsync(PolicyDTO policyDTO)
    {
        if (policyDTO == null)
        {
            throw new ArgumentNullException(nameof(policyDTO));
        }

        // Map DTO to entity
        Policy policy = _mapper.Map<Policy>(policyDTO);

        // Add new policy using UnitOfWork
        await _unitOfWork.Policy.AddAsync(policy);

        // Save changes
        return await _unitOfWork.SaveChangeAsync();
    }

    public async Task<bool> UpdatePolicyAsync(PolicyDTO policyDTO)
    {
        if (policyDTO == null)
        {
            throw new ArgumentNullException(nameof(policyDTO));
        }

        // Retrieve the existing policy
        var policy = await _unitOfWork.Policy.GetByCondition(p => p.PolicyId == policyDTO.PolicyId);
        if (policy == null)
        {
            return false;
        }

        // Update the entity's properties
        policy.Description = policyDTO.Description;
        policy.PercentageRefund = policyDTO.PercentageRefund;

        // Update policy using UnitOfWork
        _unitOfWork.Policy.Update(policy);

        // Save changes
        return await _unitOfWork.SaveChangeAsync();
    }

    public async Task<bool> DeletePolicyAsync(Guid policyId)
    {
        // Retrieve the existing policy
        var policy = await _unitOfWork.Policy.GetByCondition(p => p.PolicyId == policyId);
        if (policy == null)
        {
            return false;
        }

        // Delete the policy using UnitOfWork
        _unitOfWork.Policy.Delete(policy);

        // Save changes
        return await _unitOfWork.SaveChangeAsync();
    }
}
