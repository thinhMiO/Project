$(function () {

    $("#contactForm input, #contactForm textarea").jqBootstrapValidation({
        preventSubmit: true,
        submitError: function ($form, event, errors) {
        },
        submitSuccess: function ($form, event) {
            event.preventDefault();
            var name = $form.find("input[name='Name']").val();
            var email = $form.find("input[name='Email']").val();
            var subject = $form.find("input[name='Subject']").val();
            var message = $form.find("textarea[name='Message']").val();

            $this = $("#sendMessageButton");
            $this.prop("disabled", true);
});