using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Api_Test.Controllers
{
    public class RequestControllerTests
    {
        private readonly Mock<IRequestService> _mockRequestService;
        private readonly RequestController _requestController;

        public RequestControllerTests()
        {
            _mockRequestService = new Mock<IRequestService>();
            _requestController = new RequestController(_mockRequestService.Object);
        }

        [Fact]
        public async Task GetRequestList_ShouldReturnRequestList()
        {
            var requests = new Pagination<RequestListDTO>();
            _mockRequestService
                .Setup(s => s.GetRequestList(null, 1, 0, 10))
                .ReturnsAsync(requests);

            var response = await _requestController.GetRequestList(null, 1, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(requests, response.Value);
        }

        [Fact]
        public async Task GetRequestListById_ShouldReturnRequestListForUserId()
        {
            var userId = 1;
            var requests = new Pagination<RequestListDTO>();
            _mockRequestService
                .Setup(s => s.GetRequestList(userId, null, 0, 10))
                .ReturnsAsync(requests);

            var response = await _requestController.GetRequestList(userId, null, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(requests, response.Value);
        }

        [Fact]
        public async Task CreateRequest_ImportStatus_ShouldReturnCreatedRequest()
        {
            var newRequest = new RequestCreateDTO();
            var requestType = RequestType.Import;
            _mockRequestService
                .Setup(s => s.CreateRequest(requestType, false, newRequest))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _requestController.CreateRequest(requestType, false, newRequest) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task CreateRequest_ExportStatus_ShouldReturnCreatedRequest()
        {
            var newRequest = new RequestCreateDTO();
            var requestType = RequestType.Export;
            _mockRequestService
                .Setup(s => s.CreateRequest(requestType, false, newRequest))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _requestController.CreateRequest(requestType, false, newRequest) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task SendRequest_ShouldReturnSuccessMessage()
        {
            var requestId = 1;
            string successMessage = "Request sent successfully.";
            _mockRequestService.Setup(s => s.SendRequest(requestId)).ReturnsAsync(successMessage);

            var response = await _requestController.SendRequest(requestId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(successMessage, response.Value);
        }

        [Fact]
        public async Task CancelRequest_ShouldReturnCancellationMessage()
        {
            var requestId = 1;
            var cancellationReason = "Changed my mind.";
            string cancelMessage = "Request cancelled successfully.";
            _mockRequestService
                .Setup(s => s.CancelRequest(requestId, cancellationReason))
                .ReturnsAsync(cancelMessage);

            var response = await _requestController.CancelRequest(requestId, cancellationReason) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(cancelMessage, response.Value);
        }

        [Fact]
        public async Task UpdateRequest_ShouldReturnUpdatedRequest()
        {
            var requestId = 1;
            var updatedRequest = new RequestCreateDTO();
            _mockRequestService.Setup(s => s.UpdateRequest(requestId, updatedRequest)).ReturnsAsync(ResponseMessage.Success);

            var response = await _requestController.UpdateRequest(requestId, updatedRequest) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateRequestStatus_ShouldReturnUpdatedStatusMessage()
        {
            var requestId = 1;
            var status = RequestStatus.Completed;
            _mockRequestService
                .Setup(s => s.UpdateRequestStatus(requestId, status))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _requestController.UpdateRequestStatus(requestId, status) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteRequest_ShouldReturnDeleteMessage()
        {
            var requestId = 1;
            _mockRequestService.Setup(s => s.DeleteRequest(requestId)).ReturnsAsync(ResponseMessage.Success);

            var response = await _requestController.DeleteRequest(requestId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
