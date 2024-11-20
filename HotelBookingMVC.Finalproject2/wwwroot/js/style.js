const body = document.body;
let switch_mode = document.querySelector('#switch-mode i');
let switch_mode_1 = document.querySelector('#switch-mode_1 i');

let mode = localStorage.getItem('darkmode');
if (mode == 'true') {
  body.classList.add('dark');
  switch_mode.className = 'bi bi-brightness-high-fill';
  switch_mode_1.className = 'bi bi-brightness-high-fill';
}

document.getElementById('switch-mode').onclick = function () {
  let mode = body.classList.toggle('dark');
  switch_mode.classList.toggle('bi-moon-stars-fill');
  switch_mode.classList.toggle('bi-brightness-high-fill');
  // save mode
  localStorage.setItem('darkmode', mode);
};

document.getElementById('switch-mode_1').onclick = function () {
  let mode = body.classList.toggle('dark');
  switch_mode_1.classList.toggle('bi-moon-stars-fill');
  switch_mode_1.classList.toggle('bi-brightness-high-fill');
  // save mode
  localStorage.setItem('darkmode', mode);
};

let iconCart = document.querySelector('.iconCart');
let cart = document.querySelector('.cart');
let container = document.querySelector('.container');
let close = document.querySelector('.close');
if (cart) {
  cart.style.right = '-100%';
}

iconCart.addEventListener('click', function () {
  console.log('abc', cart.style.right);
  if (cart.style.right == '-100%') {
    cart.style.right = '0';
  } else {
    cart.style.right = '-100%';
  }
});
close.addEventListener('click', function () {
  cart.style.right = '-100%';
});

// back to top
(function () {
  // Back to Top - by CodyHouse.co
  var backTop = document.getElementsByClassName('js-cd-top')[0],
    offset = 300, // browser window scroll (in pixels) after which the "back to top" link is shown
    offsetOpacity = 1200, //browser window scroll (in pixels) after which the "back to top" link opacity is reduced
    scrollDuration = 100,
    scrolling = false;

  if (backTop) {
    //update back to top visibility on scrolling
    window.addEventListener('scroll', function (event) {
      if (!scrolling) {
        scrolling = true;
        !window.requestAnimationFrame
          ? setTimeout(checkBackToTop, 250)
          : window.requestAnimationFrame(checkBackToTop);
      }
    });

    //smooth scroll to top
    backTop.addEventListener('click', function (event) {
      event.preventDefault();
      !window.requestAnimationFrame
        ? window.scrollTo(0, 0)
        : Util.scrollTo(0, scrollDuration);
    });
  }

  function checkBackToTop() {
    var windowTop = window.scrollY || document.documentElement.scrollTop;
    windowTop > offset
      ? Util.addClass(backTop, 'cd-top--is-visible')
      : Util.removeClass(backTop, 'cd-top--is-visible cd-top--fade-out');
    windowTop > offsetOpacity && Util.addClass(backTop, 'cd-top--fade-out');
    scrolling = false;
  }
})();
