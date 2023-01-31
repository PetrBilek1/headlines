﻿using Headlines.ORM.Core.Entities;
using PBilek.ORM.Core.DAO;

namespace Headlines.BL.DAO
{
    public interface IArticleDAO : IDAO<Article, long>
    {
        Task<List<Article>> GetByUrlIdsAsync(string[] urlIds, CancellationToken cancellationToken);
        Task<List<Article>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Article>> GetByFiltersSkipTakeAsync(int skip, int take, string currentTitlePrompt, CancellationToken cancellationToken, long[]? articleSources = null, DateTime? from = null, DateTime? to = null);
        Task<long> GetCountByFiltersAsync(string currentTitlePrompt, CancellationToken cancellationToken, long[]? articleSources = null, DateTime? from = null, DateTime? to = null);
    }
}