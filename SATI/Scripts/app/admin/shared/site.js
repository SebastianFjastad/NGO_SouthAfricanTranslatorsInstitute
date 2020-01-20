$(function() {
    $('body').on('click', '.deleteBtn', function(e) {
        if (!confirm("Are you sure you want to delete this?")) {
            e.preventDefault();
        }
    });
})