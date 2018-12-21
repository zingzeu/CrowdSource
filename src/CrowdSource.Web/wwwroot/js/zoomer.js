(function($) {

   $(document).ready(function () {
     var item = $('.groupImg');
     console.log(item);
     item.magnify({
         speed:0, 
         src: item.attr('src')
     });
   });
}
(jQuery));