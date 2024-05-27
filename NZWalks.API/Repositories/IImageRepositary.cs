using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IImageRepositary
    {
        Task<Image> Upload(Image image);
    }
}
