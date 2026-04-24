using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplate
{
    public Guid Id { get; set; }
    public Guid? ParentFixedTaskTemplateId { get; set; }
    public Guid SeriesClosedListItemId { get; set; }
    public Guid CategoryClosedListItemId { get; set; }
    public int SerialNumber { get; set; }
    public int? SubTaskSerialNumber { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Notes { get; set; }
    public int DefaultWeeksBeforeExerciseStart { get; set; }
    public int DefaultDaysBeforeExerciseStart { get; set; }
    public bool IsSubTaskTemplate { get; set; }
    public bool IsActive { get; set; }
    public bool SupportsRelativeDueCompression { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(ParentFixedTaskTemplateId))]
    public FixedTaskTemplate? ParentFixedTaskTemplate { get; set; }

    [ForeignKey(nameof(SeriesClosedListItemId))]
    public required ClosedListItem SeriesClosedListItem { get; set; }

    [ForeignKey(nameof(CategoryClosedListItemId))]
    public required ClosedListItem CategoryClosedListItem { get; set; }

    [InverseProperty(nameof(ParentFixedTaskTemplate))]
    public required List<FixedTaskTemplate> SubTaskTemplates { get; set; }

    [InverseProperty("FixedTaskTemplate")]
    public required List<FixedTaskTemplateInfluencerLink> Influencers { get; set; }

    [InverseProperty("FixedTaskTemplate")]
    public required List<FixedTaskTemplateDependency> Dependencies { get; set; }

    [InverseProperty("DependsOnTemplate")]
    public required List<FixedTaskTemplateDependency> DependedOnByTemplates { get; set; }
}
