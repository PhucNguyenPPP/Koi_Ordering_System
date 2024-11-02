public interface IPolicyService
{
    Task<List<PolicyDTO>> GetAllPoliciesAsync();
    Task<PolicyDTO> GetPolicyByIdAsync(Guid policyId);
    Task<bool> AddPolicyAsync(PolicyDTO policyDTO);
    Task<bool> UpdatePolicyAsync(PolicyDTO policyDTO);
    Task<bool> DeletePolicyAsync(Guid policyId);
    Task<List<PolicyDTO>> GetPolicyByFarm(Guid farmId);
}
