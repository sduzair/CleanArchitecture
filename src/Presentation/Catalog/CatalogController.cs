using Microsoft.AspNetCore.Authorization;

using Presentation.Common;

namespace Presentation.Catalog;

[Authorize]
public sealed class CatalogController : ApiControllerBase
{
}
