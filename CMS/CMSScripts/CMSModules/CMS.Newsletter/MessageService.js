/**
 * Service for showing the information messages in Email builder.
 */
cmsdefine(['jQuery', 'CMS/MessageService'], function ($, baseService) {

  function showInfoRight(msg) {
      $('.alert-info-floating-right').removeClass('hidden')
          .find('.alert-label')
          .text(msg);
  }

  function hideInfoRight() {
      $('.alert-info-floating-right').addClass('hidden')
          .find('.alert-label')
          .text(" ");
  }

  return {
    showSuccess: baseService.showSuccess,
    showError: baseService.showError,
    showInfo: baseService.showInfo,
    showInfoRight: showInfoRight,
    hideInfoRight: hideInfoRight
  };
});