var doc = document;

var toggleBtnBurger = doc.querySelector('#burgerBtn'),
  menuList = doc.querySelector('#burgerList');

var toggleBtnOurSkills = doc.querySelectorAll('.ourSkills__itemBtn');

var logoBtn = doc.querySelectorAll('.logoBtn');

var callReqNotif = doc.querySelector('.callReq__notif');

var submitBtn = doc.querySelector('.submitBtn'),
  contactUsInputs = doc.querySelectorAll('.contactUs input'),
  contactUsTextarea = doc.querySelector('.contactUs textarea');

var callBtn = doc.querySelector('.callBackBtn'),
  callWindow = doc.querySelector('.callBackWindow'),
  callBtnClose = callWindow.querySelector('.callBackWindow__exit'),
  callWindowBtn = callWindow.querySelector('.callBackWindow__btn'),
  callWindowInputs = callWindow.querySelectorAll('.callBackWindow__input'),
  callWindowTextarea = callWindow.querySelector('.callBackWindow__textarea');

var modalWindowTY = doc.querySelector('.modalWindowTY');

var sortBtns = doc.querySelectorAll('.ourWorksBtn'),
  workCards = doc.querySelectorAll('.workCard');

var sliderBlock = doc.querySelector('.slider__content');
var sliderImgArr = ['1.png', '2.png', '3.png', '4.png', '5.png'];

var sliderItems = doc.querySelectorAll('.sliderItem'),
  countSlides = sliderItems.length,
  sliderBtns = doc.querySelectorAll('.sliderBtns__item'),
  dotsItems = doc.querySelectorAll('.sliderDots__item'),
  activeSlideIndex = 0;

var header = doc.querySelector('.mainHeader');

document.onscroll = function() {
  header.classList.remove('mainHeader-scrolled');
  if (header.getBoundingClientRect().top + pageYOffset > 100) {
    header.classList.add('mainHeader-scrolled');
  }
}

for (var i = 0; i < sliderBtns.length; i++) {
  sliderBtns[i].onclick = function() {
    if (this.classList.contains('sliderBtns__item-next')) {
      showNextSlide();
    } else {
      showPrevSlide();
    }
  }
}

for (var i = 0; i < dotsItems.length; i++) {
  dotsItems[i].onclick = function() {
    activeSlideIndex = this.getAttribute('data-index') - 1;
    showNextSlide();
  }
}

for (var i = 0; i < dotsItems.length; i++) {
  dotsItems[i].onlclick = function() {
    console.log('hi');
    clearActiveSlide();
    showActiveSlide(this.getAttribute('data-index'));
  }
}


function showNextSlide() {
  if (activeSlideIndex < countSlides - 1) {
    activeSlideIndex++;
  } else {
    activeSlideIndex = 0;
  }
  clearActiveSlide();
  showActiveSlide(activeSlideIndex);
}

function showPrevSlide() {
  if (activeSlideIndex > 0) {
    activeSlideIndex--;
  } else {
    activeSlideIndex = countSlides - 1;
  }
  clearActiveSlide();
  showActiveSlide(activeSlideIndex);
}

function clearActiveSlide() {
  for (var i = 0; i < countSlides; i++) {
    sliderItems[i].classList.remove('sliderItem-active');
    dotsItems[i].classList.remove('sliderDots__item-active');
  }
}

function showActiveSlide(index) {
  sliderItems[+index].classList.add('sliderItem-active');
  dotsItems[+index].classList.add('sliderDots__item-active');
}

for (var i = 0; i < sortBtns.length; i++) {
  sortBtns[i].onclick = function() {
    for (var j = 0; j < sortBtns.length; j++) {
      sortBtns[j].classList.remove('ourWorksBtn-active');
    }
    for (var i = 0; i < workCards.length; i++) {
      workCards[i].classList.remove('hidden');
    }
    this.classList.add('ourWorksBtn-active');
    for (var i = 0; i < workCards.length; i++) {
      if (workCards[i].getAttribute('data-type') != this.getAttribute('data-type')) {
        workCards[i].classList.add('hidden');
      }
    }
    if (this.getAttribute('data-type') == 'all') {
      for (var i = 0; i < workCards.length; i++) {
        workCards[i].classList.remove('hidden');
      }
    }
  }
}

callBtn.onclick = function() {
  callWindow.classList.remove('hidden', 'fadeOut');
  callWindow.classList.add('fadeIn')
}


callWindowBtn.onclick = function(e) {
  e.preventDefault();
  callWindowInputs[0].classList.remove('empty-field');
  callWindowInputs[1].classList.remove('empty-field');
  callWindowTextarea.classList.remove('empty-field');
  var ready = true;
  if (callWindowInputs[0].value == '') {
    callWindowInputs[0].classList.add('empty-field');
    ready = false;
  }
  if (callWindowInputs[1].value == '') {
    callWindowInputs[1].classList.add('empty-field');
    ready = false;
  }
  if (callWindowTextarea.value == '') {
    callWindowTextarea.classList.add('empty-field');
    ready = false;
  }
  if (ready) {
    callWindow.classList.remove('fadeIn');
    callWindow.classList.add('fadeOut');
    setTimeout(function() {
      callWindow.classList.add('hidden');
    }, 1000)
    callWindowInputs[0].value = '';
    callWindowInputs[1].value = '';
    callWindowTextarea.value = '';
    setTimeout(function() {
      showModalWindowTY();
    }, 1200)
  }
}

callBtnClose.onclick = function(e) {
  e.preventDefault();
  callWindow.classList.remove('fadeIn');
  callWindow.classList.add('fadeOut');
  setTimeout(function() {
    callWindow.classList.add('hidden');
  }, 1000)
  callWindowInputs[0].value = '';
  callWindowInputs[1].value = '';
  callWindowTextarea.value = '';
  callWindowInputs[0].classList.remove('empty-field');
  callWindowInputs[1].classList.remove('empty-field');
  callWindowTextarea.classList.remove('empty-field');
}

//submitBtn.onclick = function(e) {
//  e.preventDefault();
//  var ready = true;
//  contactUsInputs[0].classList.remove('empty-field');
//  contactUsInputs[1].classList.remove('empty-field');
//  contactUsTextarea.classList.remove('empty-field');

//  if (contactUsInputs[0].value == '') {
//    contactUsInputs[0].classList.add('empty-field');
//    ready = false;
//  }
//  if (contactUsInputs[1].value == '') {
//    contactUsInputs[1].classList.add('empty-field');
//    ready = false;
//  }
//  if (contactUsTextarea.value == '') {
//    contactUsTextarea.classList.add('empty-field');
//    ready = false;
//  }
//  if (ready) {
//    contactUsInputs[0].value = '';
//    contactUsInputs[1].value = '';
//    contactUsTextarea.value = '';
//    setTimeout(function() {
//      showModalWindowTY();
//    }, 1200)
//  }
//}

setInterval(function() {
  callReqNotif.classList.remove('animated', 'lightSpeedOut');
  callReqNotif.classList.remove('hidden');
  callReqNotif.classList.add('animated', 'lightSpeedIn');
  setTimeout(function() {
    callReqNotif.classList.remove('animated', 'lightSpeedIn');
    callReqNotif.classList.add('animated', 'lightSpeedOut');
    setTimeout(function() {
      callReqNotif.classList.add('hidden');
    }, 1000)
  }, 5000)
}, 120000)


toggleBtnBurger.onclick = function() {
  menuList.classList.toggle('burgerList-active');
  toggleBtnBurger.classList.toggle('burgerBtn-active');
}

for (var i = 0; i < toggleBtnOurSkills.length; i++) {
  toggleBtnOurSkills[i].onclick = function() {
    ourSkillsActive(this);
  }
}

// for (var i = 0; i < logoBtn.length; i++) {
//   logoBtn[i].onclick = function() {
//     logoBtnActive(this);
//   }
// }


function ourSkillsActive(Element) {
  Element.classList.toggle('btn-active');
  Element.parentElement.querySelector('.ourSkills__item-text').classList.toggle('text-active');
}

function logoBtnActive(obj) {
  obj.classList.add('hinge');
  setTimeout(function() {
    obj.setAttribute('style', 'opacity:0;');
  }, 2000)
}

function showModalWindowTY() {
  modalWindowTY.classList.remove('fadeOut');
  modalWindowTY.classList.add('fadeIn');
  modalWindowTY.classList.remove('hidden');
  setTimeout(function() {
    modalWindowTY.classList.remove('fadeIn');
    modalWindowTY.classList.add('fadeOut');
    setTimeout(function() {
      modalWindowTY.classList.add('hidden');
    }, 1000)
  }, 3000)
}
