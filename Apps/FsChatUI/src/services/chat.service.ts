import { GenericResponse } from './../models/generic-response';
import { environment } from './../environments/environment';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

@Injectable({
	providedIn: 'root'
})
export class ChatService {

	private API_URL = environment.publicApiBaseUrl;
	private API_ENDPOINT_BASE = `${this.API_URL}/chat`;

	constructor(
		private readonly http: HttpClient) {
	}

	public startSession(): Observable<GenericResponse> {
		return this.http.post<GenericResponse>(`${this.API_ENDPOINT_BASE}/start-session`, null).pipe(
			catchError(this.handleError('startSession', []))
		);
  }
  
  public keepAliveSession(sessionId: string): Observable<void> {
		return this.http.post<void>(`${this.API_ENDPOINT_BASE}/keep-alive-session/${sessionId}`, null).pipe(
			catchError(this.handleError('keepAliveSession', []))
		);
	}

  public endSession(sessionId: string): Observable<void> {
		return this.http.post<void>(`${this.API_ENDPOINT_BASE}/end-session/${sessionId}`, null).pipe(
			catchError(this.handleError('endSession', []))
		);
  }

	/**
	 * Handle Http operation that failed.
	 * Let the app continue.
	 * @param operation - name of the operation that failed
	 * @param result - optional value to return as the observable result
	 */
	private handleError<T>(operation = 'operation', result?: any) {
		return (error: any): Observable<any> => {
			// TODO: send the error to remote logging infrastructure
			console.error(error); // log to console instead

			throwError(result);
			// Let the app keep running by returning an empty result.
			throw new Error(result);
			// return from(result);
		};
	}
}