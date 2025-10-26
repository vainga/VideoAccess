using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoAccess.Models.Entities;

namespace VideoAccess.Data
{
    /// <summary>
    /// Интерфейс репозитория для работы с видеофайлами в базе данных.
    /// Определяет стандартные операции CRUD и дополнительные методы для работы с видео.
    /// </summary>
    public interface IVideoRepository
    {
        /// <summary>
        /// Создает новую запись о видео в базе данных.
        /// </summary>
        /// <param name="video">Объект видео для создания.</param>
        /// <returns>Созданный объект видео с присвоенным идентификатором.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается если передан null.</exception>
        Task<VideoEntity> CreateAsync(VideoEntity video);

        /// <summary>
        /// Получает видео по его уникальному идентификатору.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <returns>Найденный объект видео или null если не найден.</returns>
        Task<VideoEntity?> GetByIdAsync(Guid videoId);

        /// <summary>
        /// Получает все видео из базы данных отсортированные по дате загрузки (сначала новые).
        /// </summary>
        /// <returns>Список всех видео.</returns>
        Task<List<VideoEntity>> GetAllAsync();

        /// <summary>
        /// Получает все видео определенного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список видео пользователя отсортированный по дате загрузки.</returns>
        Task<List<VideoEntity>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Находит видео по имени файла.
        /// </summary>
        /// <param name="fileName">Имя файла для поиска.</param>
        /// <returns>Найденный объект видео или null если не найден.</returns>
        Task<VideoEntity?> GetByFileNameAsync(string fileName);

        /// <summary>
        /// Обновляет информацию о видео в базе данных.
        /// </summary>
        /// <param name="video">Объект видео с обновленными данными.</param>
        /// <returns>Обновленный объект видео или null если видео не найдено.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается если передан null.</exception>
        Task<VideoEntity?> UpdateAsync(VideoEntity video);

        /// <summary>
        /// Обновляет только имя файла видео.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <param name="newFileName">Новое имя файла.</param>
        /// <returns>Обновленный объект видео или null если видео не найдено.</returns>
        Task<VideoEntity?> UpdateFileNameAsync(Guid videoId, string newFileName);

        /// <summary>
        /// Удаляет видео из базы данных по идентификатору.
        /// </summary>
        /// <param name="videoId">Идентификатор видео для удаления.</param>
        /// <returns>True если удаление прошло успешно, false если видео не найдено.</returns>
        Task<bool> DeleteAsync(Guid videoId);

        /// <summary>
        /// Удаляет все видео определенного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Количество удаленных записей.</returns>
        Task<int> DeleteByUserIdAsync(Guid userId);

        /// <summary>
        /// Проверяет существует ли видео с указанным идентификатором.
        /// </summary>
        /// <param name="videoId">Идентификатор видео для проверки.</param>
        /// <returns>True если видео существует, иначе false.</returns>
        Task<bool> ExistsAsync(Guid videoId);

        /// <summary>
        /// Получает видео с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы (начинается с 1).</param>
        /// <param name="pageSize">Количество элементов на странице.</param>
        /// <returns>Кортеж содержащий список видео и общее количество записей.</returns>
        Task<(List<VideoEntity> Videos, int TotalCount)> GetPaginatedAsync(int page, int pageSize);

        /// <summary>
        /// Получает общее количество видео в базе данных.
        /// </summary>
        /// <returns>Общее количество видео.</returns>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Получает количество видео определенного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Количество видео пользователя.</returns>
        Task<int> GetCountByUserIdAsync(Guid userId);
    }
}
