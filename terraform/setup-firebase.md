# Firebase Project Setup Guide

The Terraform configuration requires an existing Google Cloud project with Firebase enabled. Follow these steps:

## Option 1: Enable Firebase via Console (Recommended)

1. **Go to Firebase Console**: https://console.firebase.google.com/
2. **Add Project**: Click "Add project" or "Create a project"
3. **Select existing GCP project**: Choose your existing Google Cloud project
4. **Enable Firebase**: Follow the setup wizard to enable Firebase for your project
5. **Enable Hosting**: Go to Hosting section and click "Get started"

## Option 2: Enable Firebase via CLI

```bash
# Install Firebase CLI if not already installed
npm install -g firebase-tools

# Login to Firebase
firebase login

# Initialize Firebase in your project
firebase projects:addfirebase YOUR_PROJECT_ID

# Enable Hosting
firebase init hosting
```

## Option 3: Use Terraform to Initialize Firebase (Advanced)

If you have the necessary permissions, uncomment the `google_firebase_project` resource in `main.tf`:

```hcl
resource "google_firebase_project" "default" {
  provider = google-beta
  project  = var.project_id

  depends_on = [
    google_project_service.firebase,
    google_project_service.firebasehosting,
    google_project_service.identitytoolkit,
  ]
}
```

## Required Permissions

For Terraform to work, your account needs:
- `firebase.projects.create` (if creating new Firebase project)
- `serviceusage.services.enable`
- `firebase.projects.get`
- `firebasehosting.sites.create`

## After Setup

Once Firebase is enabled in your project, run:
```bash
terraform plan
terraform apply
```
