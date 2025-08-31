using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Infrastructure.Database.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditEntity auditable) continue;

            if (entry.State == EntityState.Added)
            {
                auditable.CreatedAtUtc = now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                auditable.LastModifiedAtUtc = now;
            }
        }
    }
}