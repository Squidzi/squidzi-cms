using Squidzi.Domain.SquidexRepo.Models;
using Squidzi.Domain.SquidexRepo.Models.Pages;
using Squidzi.Models.CMS;
using Squidzi.Models.CMS.Pages;

namespace Squidzi.Services.CMS.Mappers.Contracts
{
    public interface ICMSMapper
    {
        BlogPost MapToBlogPost(BlogPostEntity model, AuthorEntity author);

        Global MapToGlobal(GlobalEntity model);

        Project MapToProject(ProjectEntity model);

        PageAbout MapToPage_About(PageAboutEntity model);

        PageContact MapToPage_Contact(PageContactEntity model);

        PageIndex MapToPage_Index(PageIndexEntity model);

        PageProjects MapToPage_Projects(PageProjectsEntity model);

        PageSearchResults MapToPage_SearchResults(PageSearchResultsEntity model);
    }
}
