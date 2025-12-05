using AutoMapper;
using FluentValidation;
using Grpc.Core;
using MediatR;

using Librarymanagement;
using LibraryManagement.Application.Authors.CreateAuthor;
using LibraryManagement.Application.Authors.GetAuthor;
using LibraryManagement.Application.Authors.GetAuthors;
using LibraryManagement.Application.Authors.UpdateAuthor;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Shared.Exceptions;
using LibraryManagement.Application.Authors.DeleteAuthor;

public class GrpcAuthorService : AuthorService.AuthorServiceBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GrpcAuthorService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public override async Task<AuthorGetResponse> GetAuthor(AuthorGetRequest request, ServerCallContext context)
    {
        try
        {
            var author = await _mediator.Send(new GetAuthor(request.AuthorId));
            return new AuthorGetResponse
            {
                Author = _mapper.Map<AuthorResponse>(author)
            };
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AuthorListResponse> GetAuthors(AuthorPageRequest request, ServerCallContext context)
    {
        try
        {
            var searchAuthorCommand = _mapper.Map<SearchAuthorCommand>(request.SearchRequest);
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var (totalCount, numberOfPages, searchResultDtos) = await _mediator.Send(new GetAuthors(searchAuthorCommand, pageSize, pageNumber));
            var searchResult = _mapper.Map<IEnumerable<AuthorResponse>>(searchResultDtos);

            var response = new AuthorListResponse
            {
                TotalCount = totalCount,
                NumberOfPages = numberOfPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            response.Authors.AddRange(searchResult);

            return response;
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AuthorResponse> CreateAuthor(CreateAuthorRequest request, ServerCallContext context)
    {
        try
        {
            var createAuthorCommand = _mapper.Map<CreateAuthorCommand>(request);

            var newAuthorDto = await _mediator.Send(new CreateAuthor(createAuthorCommand));
            return _mapper.Map<AuthorResponse>(newAuthorDto);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AuthorResponse> UpdateAuthor(UpdateAuthorRequest request, ServerCallContext context)
    {
        try
        {
            var updateAuthorCommand = _mapper.Map<UpdateAuthorCommand>(request);
            long AuthorId = request.AuthorId;

            var updatedAuthorDto = await _mediator.Send(new UpdateAuthor(updateAuthorCommand, AuthorId));
            return _mapper.Map<AuthorResponse>(updatedAuthorDto);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AuthorDeleteResponse> DeleteAuthor(AuthorDeleteRequest request, ServerCallContext context)
    {
        try
        {
            await _mediator.Send(new DeleteAuthor(request.AuthorId));
            return new AuthorDeleteResponse { Message = $"Author {request.AuthorId} was successfully deleted." };
        }
        catch(ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
