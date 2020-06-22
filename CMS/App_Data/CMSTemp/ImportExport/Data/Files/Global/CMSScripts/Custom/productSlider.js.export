function productSlider(stepLength,width,height,container,buttonNext,buttonPrev)
{
  if (jQuery(container+" ul li").length>stepLength)
  {
    overStep=jQuery(container+" ul li").length%stepLength;
    blanks=0;
    if (overStep>0) blanks=stepLength-overStep;
    for (i=0;i<blanks;i++) 
      jQuery(container+" ul").append("<li></li>");
    jQuery(container).jCarouselLite({
      'btnNext': buttonNext,
      'btnPrev': buttonPrev,
      'width': width,
      'height': height,
      'circular': false,
      'visible': stepLength,
      'scroll': stepLength
    }); 
    jQuery(container).show();
    jQuery(container).mouseover(function(){
      jQuery(buttonNext).show();
      jQuery(buttonPrev).show();
    });
    jQuery(container).mouseout(function(){
      jQuery(buttonNext).hide();
      jQuery(buttonPrev).hide();
    });
  }
}