import { ChatService } from './../../services/chat.service';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.css']
})
export class ChatWindowComponent implements OnInit, OnDestroy {

  public status: string = '';
  public error: string = '';
  public sessionId: string;

  private lastSubscription: Subscription;
  private subscriptions: Subscription[] = [];

  constructor(
    private readonly chatService: ChatService,
    private readonly cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.startPollingWhenSessionIdIsAvailable();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  startSession() {
    this.subscriptions.push(this.chatService.startSession().subscribe(
      response => {
        if (response.success) {
          this.sessionId = response.successMessage;
          this.status = `Session ${this.sessionId} queued`;
        } else {
          this.error = response.errorMessage;
        }
        this.cdr.markForCheck();
      },
      error => {
        alert('Could not start session');
      }
    ));
  }

  endSession() {
    this.subscriptions.push(this.chatService.endSession(this.sessionId).subscribe(
      response => {
        this.clearState();
        this.cdr.markForCheck();
      },
      error => {
        alert('Could not end session');
      }
    ));
  }

  // TODO this should be improved and we should use sockets instead of polling. 
  // This way it is much faster and the server has much less load
  startPollingWhenSessionIdIsAvailable() {
    setInterval(() => {
			if (this.lastSubscription) {
        this.lastSubscription.unsubscribe();
      }
      if (this.sessionId) {
        this.lastSubscription = this.chatService.keepAliveSession(this.sessionId).subscribe();
      }
		}, 1000);
  }

  clearState() {
    this.sessionId = null;
    this.status = '';
    this.error = '';
  }
}
