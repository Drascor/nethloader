﻿// Search for dialog triggers
var dialogTriggers;

const init = () => {
  findTriggers();
  addListeners();
}

/*
 * Find all the dialog triggers, with the 'data-dialogtrigger' attribute
 */
const findTriggers = () => {
  dialogTriggers = document.querySelectorAll("[data-dialogtrigger]");
}

/*
 * Add event listeners to open the dialog respectively
 */
const addListeners = () => {
  for (let i = dialogTriggers.length - 1; i >= 0; i--) {
    dialogTriggers[i].addEventListener('click', openDialog);
  }
}

/*
 * Open the dialog element
 */
const openDialog = (event) => {
  let currentTarget = event.currentTarget;
  let dialogWrapper = document.querySelector(`[data-dialogid=${currentTarget.dataset.dialogtrigger}]`);
  dialogWrapper.classList.add('is-active'); // Overlay
  dialogWrapper.children[0].classList.add('is-active');  // Modal

  let closeButton = document.querySelector(`[data-closedialog=${currentTarget.dataset.dialogtrigger}]`);
  closeButton.addEventListener('click', () => {
    dialogWrapper.classList.remove('is-active'); // Overlay
    dialogWrapper.children[0].classList.remove('is-active'); // Modal
  })
}

export default {
  init
}
