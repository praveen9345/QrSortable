namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;
    using Microsoft.Maui.Controls;
    using System.Collections.Generic;

    /// <summary>
    /// ItemDetailView with smooth zoom functionality for iOS and Android
    /// Features:
    /// - Pinch to zoom (1x to 4x)
    /// - Pan when zoomed in
    /// - Double-tap to toggle zoom
    /// - Smart zoom snapping
    /// - Smooth animations (250ms)
    /// - Gentle bounce-back effect
    /// </summary>
    public partial class ItemDetailView : BaseView
    {
        #region Fields

        // Track zoom state for each image instance
        private readonly Dictionary<Image, ImageZoomState> _imageStates = new Dictionary<Image, ImageZoomState>();

        // Zoom configuration
        private const double MIN_SCALE = 1.0;
        private const double MAX_SCALE = 4.0;
        private const double DOUBLE_TAP_SCALE = 2.0;

        // Animation settings - works smoothly on both iOS and Android
        private const uint ANIMATION_DURATION = 250;  // 250ms feels natural on both platforms
        private readonly Easing ZOOM_EASING = Easing.CubicOut;  // Natural deceleration

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ItemDetailView class
        /// </summary>
        public ItemDetailView(ItemDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Existing Methods (Preserved)

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            cameraView.HeightRequest = height;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            cameraView.CaptureNextFrame = true;
        }

        #endregion

        #region Smooth Zoom Gesture Handlers

        /// <summary>
        /// Handles pinch gesture for smooth zooming
        /// Works on both iOS (UIPinchGestureRecognizer) and Android (ScaleGestureDetector)
        /// </summary>
        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (sender is not Image image) return;

            // Get or create state for this image
            if (!_imageStates.ContainsKey(image))
            {
                _imageStates[image] = new ImageZoomState();
            }

            var state = _imageStates[image];

            switch (e.Status)
            {
                case GestureStatus.Started:
                    // Cancel any ongoing animations for responsiveness
                    image.AbortAnimation("ScaleAnimation");
                    image.AbortAnimation("TranslateXAnimation");
                    image.AbortAnimation("TranslateYAnimation");

                    // Store starting scale
                    state.StartScale = state.CurrentScale;
                    image.AnchorX = 0.5;
                    image.AnchorY = 0.5;
                    break;

                case GestureStatus.Running:
                    // Calculate and apply new scale (instant for responsiveness)
                    var newScale = state.StartScale * e.Scale;
                    state.CurrentScale = Math.Clamp(newScale, MIN_SCALE, MAX_SCALE);
                    image.Scale = state.CurrentScale;

                    // Disable carousel swiping when zoomed
                    UpdateCarouselSwipeEnabled(state.CurrentScale <= MIN_SCALE);
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    // Smooth zoom out if at minimum
                    if (state.CurrentScale <= MIN_SCALE)
                    {
                        ResetImageTransformAnimated(image, state);
                        UpdateCarouselSwipeEnabled(true);
                    }
                    else
                    {
                        // Snap to nearest sensible zoom level
                        SnapToNearestZoomLevel(image, state);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles pan gesture for moving zoomed images
        /// Works on both iOS (UIPanGestureRecognizer) and Android (GestureDetector)
        /// </summary>
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (sender is not Image image) return;

            // Get state for this image
            if (!_imageStates.ContainsKey(image))
            {
                _imageStates[image] = new ImageZoomState();
            }

            var state = _imageStates[image];

            // Only allow panning if zoomed in
            if (state.CurrentScale <= MIN_SCALE) return;

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Calculate new position with bounds
                    double newX = state.XOffset + e.TotalX;
                    double newY = state.YOffset + e.TotalY;

                    // Calculate max pan distance
                    double maxX = Math.Max(0, (image.Width * (state.CurrentScale - 1)) / 2);
                    double maxY = Math.Max(0, (image.Height * (state.CurrentScale - 1)) / 2);

                    // Apply clamped translation
                    image.TranslationX = Math.Clamp(newX, -maxX, maxX);
                    image.TranslationY = Math.Clamp(newY, -maxY, maxY);
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    // Store final position
                    state.XOffset = image.TranslationX;
                    state.YOffset = image.TranslationY;

                    // Gentle bounce-back if out of bounds
                    AnimateToBounds(image, state);
                    break;
            }
        }

        /// <summary>
        /// Handles double-tap for quick zoom toggle with smooth animation
        /// Works on both iOS (UITapGestureRecognizer) and Android (GestureDetector)
        /// </summary>
        private async void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            if (sender is not Image image) return;

            // Get or create state for this image
            if (!_imageStates.ContainsKey(image))
            {
                _imageStates[image] = new ImageZoomState();
            }

            var state = _imageStates[image];

            // Cancel ongoing animations
            image.AbortAnimation("ScaleAnimation");
            image.AbortAnimation("TranslateXAnimation");
            image.AbortAnimation("TranslateYAnimation");

            if (state.CurrentScale > MIN_SCALE)
            {
                // Smoothly zoom out to normal
                await ResetImageTransformAnimated(image, state);
                UpdateCarouselSwipeEnabled(true);
            }
            else
            {
                // Smoothly zoom in to 2x
                state.CurrentScale = DOUBLE_TAP_SCALE;
                state.XOffset = 0;
                state.YOffset = 0;

                // Parallel animations for smooth feel
                var scaleTask = image.ScaleTo(DOUBLE_TAP_SCALE, ANIMATION_DURATION, ZOOM_EASING);
                var translateXTask = image.TranslateTo(0, image.TranslationY, ANIMATION_DURATION, ZOOM_EASING);
                var translateYTask = image.TranslateTo(image.TranslationX, 0, ANIMATION_DURATION, ZOOM_EASING);

                await Task.WhenAll(scaleTask, translateXTask, translateYTask);

                UpdateCarouselSwipeEnabled(false);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Resets image to normal state with smooth animation
        /// </summary>
        private async Task ResetImageTransformAnimated(Image image, ImageZoomState state)
        {
            state.CurrentScale = MIN_SCALE;
            state.StartScale = MIN_SCALE;
            state.XOffset = 0;
            state.YOffset = 0;

            // Smooth parallel animations
            var scaleTask = image.ScaleTo(MIN_SCALE, ANIMATION_DURATION, ZOOM_EASING);
            var translateXTask = image.TranslateTo(0, image.TranslationY, ANIMATION_DURATION, ZOOM_EASING);
            var translateYTask = image.TranslateTo(image.TranslationX, 0, ANIMATION_DURATION, ZOOM_EASING);

            await Task.WhenAll(scaleTask, translateXTask, translateYTask);
        }

        /// <summary>
        /// Snaps zoom to nearest clean level (1.0x, 1.5x, 2.0x, 3.0x, 4.0x)
        /// Prevents awkward zoom levels like 1.73x
        /// </summary>
        private async void SnapToNearestZoomLevel(Image image, ImageZoomState state)
        {
            // Define clean zoom levels
            double[] snapPoints = { 1.0, 1.5, 2.0, 3.0, 4.0 };

            // Find nearest snap point
            double nearestSnap = snapPoints
                .OrderBy(snap => Math.Abs(snap - state.CurrentScale))
                .First();

            // Only snap if close enough (within 0.3)
            if (Math.Abs(state.CurrentScale - nearestSnap) < 0.3)
            {
                state.CurrentScale = nearestSnap;
                await image.ScaleTo(nearestSnap, ANIMATION_DURATION / 2, ZOOM_EASING);
            }
        }

        /// <summary>
        /// Gently bounces image back if panned beyond valid bounds
        /// Uses SpringOut easing for natural feel
        /// </summary>
        private async void AnimateToBounds(Image image, ImageZoomState state)
        {
            // Calculate valid bounds
            double maxX = Math.Max(0, (image.Width * (state.CurrentScale - 1)) / 2);
            double maxY = Math.Max(0, (image.Height * (state.CurrentScale - 1)) / 2);

            // Calculate clamped position
            double targetX = Math.Clamp(state.XOffset, -maxX, maxX);
            double targetY = Math.Clamp(state.YOffset, -maxY, maxY);

            // If out of bounds, animate back with spring effect
            if (Math.Abs(targetX - state.XOffset) > 1 || Math.Abs(targetY - state.YOffset) > 1)
            {
                state.XOffset = targetX;
                state.YOffset = targetY;

                await image.TranslateTo(targetX, targetY, ANIMATION_DURATION / 2, Easing.SpringOut);
            }
        }

        /// <summary>
        /// Controls carousel swiping based on zoom state
        /// Disables swiping when zoomed to prevent gesture conflicts
        /// </summary>
        private void UpdateCarouselSwipeEnabled(bool enabled)
        {
            if (imageCarousel != null)
            {
                imageCarousel.IsSwipeEnabled = enabled;
            }
        }

        /// <summary>
        /// Cleans up zoom states and cancels animations
        /// Important for preventing memory leaks
        /// </summary>
        public void CleanupImageStates()
        {
            // Cancel all ongoing animations
            foreach (var kvp in _imageStates)
            {
                var image = kvp.Key;
                image.AbortAnimation("ScaleAnimation");
                image.AbortAnimation("TranslateXAnimation");
                image.AbortAnimation("TranslateYAnimation");
            }

            _imageStates.Clear();
        }

        #endregion

        #region Lifecycle Overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // States will be initialized as needed
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Clean up to prevent memory leaks
            CleanupImageStates();
        }

        #endregion
    }

    /// <summary>
    /// Tracks zoom and pan state for individual images
    /// Each image in the carousel has its own independent state
    /// </summary>
    internal class ImageZoomState
    {
        public double CurrentScale { get; set; } = 1.0;
        public double StartScale { get; set; } = 1.0;
        public double XOffset { get; set; } = 0;
        public double YOffset { get; set; } = 0;
    }
}