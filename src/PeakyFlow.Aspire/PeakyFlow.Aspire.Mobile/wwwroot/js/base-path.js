// Dynamic base path configuration
window.Blazor = window.Blazor || {};
window.Blazor.start = window.Blazor.start || function() {};

// Set base path dynamically based on environment
if (window.location.hostname.includes('github.io')) {
    // GitHub Pages deployment
    document.querySelector('base').href = '/PeakyFlow/';
} else {
    // Local development
    document.querySelector('base').href = '/';
}
