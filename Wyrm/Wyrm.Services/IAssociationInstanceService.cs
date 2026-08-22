using Wyrm.ViewModels;

namespace Wyrm.Services
{
    /// <summary>
    /// One existing link shown in the Explorer's "Associated Objects" tab - the direction-appropriate
    /// label (ForwardName if the viewed instance is the Source, ReverseName if it's the Target) plus
    /// the other end's display info.
    /// </summary>
    public record AssociatedObjectRow(int AssociationInstanceId, string DirectionLabel, int OtherObjectInstanceId, string OtherObjectTypeName, string OtherDisplayName);

    /// <summary>
    /// One selectable "other object" when creating a new association - an instance whose ObjectType
    /// matches the association's Source/Target requirement (or any instance in the repository, if that
    /// end is "Any").
    /// </summary>
    public record AssociationCandidateInstance(int InstanceId, string ObjectTypeName, string DisplayName);

    /// <summary>
    /// One creatable association for the object instance currently being viewed: which AssociationType,
    /// which direction (CurrentInstanceIsSource), the eligible other-end instances, and the association's
    /// own PropertyTypes pre-built as blank <see cref="PropertyFieldInput"/>s ready for editing.
    /// </summary>
    public record EligibleAssociationOption(int AssociationTypeId, string Label, bool CurrentInstanceIsSource, List<AssociationCandidateInstance> Candidates, List<PropertyFieldInput> PropertyFields);

    public interface IAssociationInstanceService
    {
        /// <summary>
        /// Loads the existing associations (both as Source and as Target) for an object instance, for the
        /// Explorer "Associated Objects" tab.
        /// </summary>
        Task<List<AssociatedObjectRow>> GetAssociatedObjectsAsync(int objectInstanceId);

        /// <summary>
        /// Loads every AssociationType (and direction) that the given object instance is eligible to
        /// participate in, each pre-loaded with its candidate "other end" instances and blank property
        /// fields, ready to drive the "Add Association" form without further round-trips.
        /// </summary>
        Task<List<EligibleAssociationOption>> GetEligibleAssociationsAsync(int objectInstanceId);

        /// <summary>
        /// Creates a new association instance plus any of its property values, in one unit of work.
        /// Returns the new association instance's Id.
        /// </summary>
        Task<int> CreateAsync(AssociationInstanceFormInput input, string userId);

        Task DeleteAsync(int associationInstanceId);
    }
}
