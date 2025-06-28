using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;

using GoActive.Console.Features.Export.Options;
using GoActive.Shared.Serialization.NetTopologySuite;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace GoActive.Console.Features.Export.Commands;

internal class ExportSpotCommand : Command
{
    private const string CommandName = "spot";

    public ExportSpotCommand()
        : base(CommandName, "Export spot data")
    {
    }

    internal class CommandHandler() : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var file = context.ParseResult.GetValueForOption(ExportOptions.FileOption);
            if (string.IsNullOrWhiteSpace(file))
            {
                context.Console.WriteLine($"File can't be empty or whilespaces only: {file}");
                return 1;
            }

            context.Console.WriteLine($"Exporting spot data...");

            var fileInfo = new FileInfo(file);
            var builder = new StringBuilder();
            var serializer = new GeoJsonSerializer(writeIndented: false);

#pragma warning disable CS0618 // Type or member is obsolete

            foreach (var spot in Modules.Geo.Domain.SpotAggregate.Spot.InitialData)
            {
                builder.AppendLine(serializer.SerializeFeature(SpotFeature.FromDomainSpot(spot)));
            }

            await File.WriteAllTextAsync(fileInfo.FullName, builder.ToString(), context.GetCancellationToken());

            context.Console.WriteLine($"Data exported to: {fileInfo.FullName}");
            return 0;

#pragma warning restore CS0618 // Type or member is obsolete
        }

        private sealed class SpotFeature : IFeature
        {
            private readonly Geometry _geometry;
            private readonly Envelope _boundingBox;
            private readonly IAttributesTable _attributes;

            private SpotFeature(Geometry geometry, Envelope boundingBox, IAttributesTable attributes)
            {
                _geometry = geometry;
                _boundingBox = boundingBox;
                _attributes = attributes;
            }

            public Geometry Geometry { get => _geometry; set => throw new NotImplementedException(); }
            public Envelope BoundingBox { get => _boundingBox; set => throw new NotImplementedException(); }
            public IAttributesTable Attributes { get => _attributes; set => throw new NotImplementedException(); }

            internal static SpotFeature FromDomainSpot(Modules.Geo.Domain.SpotAggregate.Spot spot)
            {
                var point = spot.LocationPoint.ToPoint();

                return new SpotFeature(
                    geometry: point,
                    boundingBox: point.EnvelopeInternal,
                    attributes: new AttributesTable
                    {
                        { "id", spot.Id.Value },
                        { "title", spot.Title.Value },
                        { "description", spot.Description },
                        { "activities", spot.Activities },
                    });
            }

        }
    }
}