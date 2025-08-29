terraform {
  required_version = ">= 1.0"
  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 5.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = "~> 5.0"
    }
  }
}

provider "google" {
  project = var.project_id
  region  = var.region
}

provider "google-beta" {
  project = var.project_id
  region  = var.region
}

# Enable required APIs
resource "google_project_service" "firebase" {
  project = var.project_id
  service = "firebase.googleapis.com"

  disable_dependent_services = false
  disable_on_destroy         = false
}

resource "google_project_service" "firebasehosting" {
  project = var.project_id
  service = "firebasehosting.googleapis.com"

  disable_dependent_services = false
  disable_on_destroy         = false
}

resource "google_project_service" "identitytoolkit" {
  project = var.project_id
  service = "identitytoolkit.googleapis.com"

  disable_dependent_services = false
  disable_on_destroy         = false
}

# Use existing Firebase project (comment out if project doesn't have Firebase enabled)
resource "google_firebase_project" "default" {
  provider = google-beta
  project  = var.project_id
  depends_on = [
    google_project_service.firebase,
    google_project_service.firebasehosting,
    google_project_service.identitytoolkit,
  ]
}

# Firebase Hosting site
resource "google_firebase_hosting_site" "peakyflow" {
  provider = google-beta
  project  = var.project_id
  site_id  = var.site_id
  
  depends_on = [
    google_project_service.firebase,
    google_project_service.firebasehosting,
  ]
}
