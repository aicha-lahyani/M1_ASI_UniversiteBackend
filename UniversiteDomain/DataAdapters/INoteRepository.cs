using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters.DataAdaptersFactory;

public interface INoteRepository : IRepository<Note>
{
    Task SaveOrUpdateAsync(Note note);
}