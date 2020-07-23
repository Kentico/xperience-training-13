using Kentico.Content.Web.Mvc;

namespace XperienceAdapter.Models
{

    /// <summary>
    /// Interface for size constrains so that we can avoid using Kentico code directly in views
    /// </summary>
    public interface IImageSizeConstraint
    {
        SizeConstraint GetSizeConstraint();
    }

    public class Width : IImageSizeConstraint
    {
        private readonly int _width;
        public Width(int width)
        {
            _width = width;
        }

        public SizeConstraint GetSizeConstraint()
        {
            return SizeConstraint.Width(_width);
        }
    }

    public class Height : IImageSizeConstraint
    {
        private readonly int _height;
        public Height(int height)
        {
            _height = height;
        }

        public SizeConstraint GetSizeConstraint()
        {
            return SizeConstraint.Height(_height);
        }
    }

    public class MaxWidthOrHeight : IImageSizeConstraint
    {
        private readonly int _maxWidthOrHeight;
        public MaxWidthOrHeight(int maxWidthOrHeight)
        {
            _maxWidthOrHeight = maxWidthOrHeight;
        }

        public SizeConstraint GetSizeConstraint()
        {
            return SizeConstraint.MaxWidthOrHeight(_maxWidthOrHeight);
        }
    }

}
