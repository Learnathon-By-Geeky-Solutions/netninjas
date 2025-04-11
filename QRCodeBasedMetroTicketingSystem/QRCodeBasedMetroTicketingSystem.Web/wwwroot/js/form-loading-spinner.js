$(document).ready(function () {
    $('.loading-form').on('submit', function () {
        const $form = $(this);
        const $submitBtn = $form.find('button[type="submit"]');
        const $btnText = $submitBtn.find('.btn-text');
        const $spinner = $submitBtn.find('#loadingSpinner');
        const loadingText = $submitBtn.data('loading-text') || 'Submitting...';

        // Show loading state
        $spinner.removeClass('d-none');
        $submitBtn.prop("disabled", true);
        $btnText.text(loadingText);

        return true; // Allow form submission
    });
});