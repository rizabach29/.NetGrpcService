using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService2;

namespace GrpcService2.Services
{
    public class UserService : ExamplePhotoService.ExamplePhotoServiceBase
    {
        public override Task<User> CreateUser(User request, ServerCallContext context)
        {
            return Task.FromResult(request);
        }

        public override Task<Empty> UploadPhoto(IAsyncStreamReader<PhotoDataBlock> requestStream, ServerCallContext context)
        {
            return base.UploadPhoto(requestStream, context);
        }

        public override async Task StreamPhotos(IAsyncStreamReader<GetPhotoRequest> requestStream, IServerStreamWriter<Photo> responseStream, ServerCallContext context)
        {
            int i = 0;
            await foreach (var photo in requestStream.ReadAllAsync())
            {
                Console.WriteLine(photo.Name);

                await responseStream.WriteAsync(new Photo()
                {
                    Name = photo.Name,
                    DisplayName = $"Photos-Name-{i}",
                });
                await Task.Delay(TimeSpan.FromSeconds(1));
                i++;
            }
        }

        public override async Task GetPhotos(Empty request, IServerStreamWriter<Photo> responseStream, ServerCallContext context)
        {
            var i = 0;

            while (i <= 100)
            {
                await responseStream.WriteAsync(new Photo()
                {
                    Name = $"Photos-{i}",
                    DisplayName = $"Photos-Name-{i}",
                });
                await Task.Delay(TimeSpan.FromSeconds(1));
                i++;
            }
        }
    }
}
