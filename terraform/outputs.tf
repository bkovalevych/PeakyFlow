output "firebase_hosting_url" {
  description = "The URL of the Firebase Hosting site"
  value       = "https://${google_firebase_hosting_site.peakyflow.site_id}.web.app"
}

output "firebase_hosting_custom_domain" {
  description = "Custom domain for Firebase Hosting (if configured)"
  value       = "https://${google_firebase_hosting_site.peakyflow.site_id}.firebaseapp.com"
}

output "project_id" {
  description = "The Google Cloud Project ID"
  value       = var.project_id
}

output "site_id" {
  description = "The Firebase Hosting site ID"
  value       = google_firebase_hosting_site.peakyflow.site_id
}
