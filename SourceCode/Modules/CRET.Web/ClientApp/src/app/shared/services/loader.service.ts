import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  isLoading: boolean = false;

  isLoadingForDesignatedAndPrioritySites: boolean = false;

  listOfUrls: string[] = [];
  constructor() {
  }

}
