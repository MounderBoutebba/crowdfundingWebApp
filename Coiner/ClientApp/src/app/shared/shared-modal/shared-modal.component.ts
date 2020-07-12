import { Component, OnInit, Input } from '@angular/core';

declare var $;

@Component({
  selector: 'shared-modal',
  templateUrl: './shared-modal.component.html',
  styleUrls: ['./shared-modal.component.css']
})
export class SharedModalComponent implements OnInit {

  @Input() modalType: string; // 'success', 'fail'
  @Input() modalTitle: string;
  @Input() modalMessage: string;

  constructor() { }

  ngOnInit() {
    $('#sharedModal').on('hidden.bs.modal', () => {
      this.modalMessage=null;
    })
  }

  openModal() {
    $('#sharedModal').modal('show');
  }

  closeModal() {
    $('#sharedModal').modal('hide');
  }

}
