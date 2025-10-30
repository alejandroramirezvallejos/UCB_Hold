import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { importProvidersFrom, LOCALE_ID } from '@angular/core';
import { AppRoutingModule } from './app/app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';

registerLocaleData(localeEs);


bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(AppRoutingModule, HttpClientModule),
     { provide: LOCALE_ID, useValue: 'es' }
  ]
}).catch(err => console.error(err));

