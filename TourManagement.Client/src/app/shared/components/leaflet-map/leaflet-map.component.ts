import {
  Component,
  AfterViewInit,
  OnDestroy,
  OnChanges,
  SimpleChanges,
  input,
  output,
  ElementRef,
  viewChild,
} from '@angular/core';
import * as L from 'leaflet';

import { KeyPoint } from '../../../core/models/tour.model';

delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
});

@Component({
  selector: 'app-leaflet-map',
  standalone: true,
  template: `<div #mapEl class="h-full w-full min-h-[350px] rounded-lg"></div>`,
})
export class LeafletMapComponent implements AfterViewInit, OnDestroy, OnChanges {
  readonly keyPoints = input<KeyPoint[]>([]);
  readonly editable = input(false);
  readonly connectPoints = input(false);
  readonly center = input<[number, number]>([45.25, 19.85]);
  readonly zoom = input(13);

  readonly mapClick = output<{ lat: number; lng: number }>();

  readonly mapEl = viewChild.required<ElementRef<HTMLDivElement>>('mapEl');

  private map: L.Map | null = null;
  private markerLayer = L.layerGroup();
  private polyline: L.Polyline | null = null;

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl().nativeElement).setView(this.center(), this.zoom());

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(this.map);

    this.markerLayer.addTo(this.map);

    if (this.editable()) {
      this.map.on('click', (e: L.LeafletMouseEvent) => {
        this.mapClick.emit({ lat: e.latlng.lat, lng: e.latlng.lng });
      });
    }

    this.renderKeyPoints();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.map && (changes['keyPoints'] || changes['connectPoints'])) {
      this.renderKeyPoints();
    }
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }

  private renderKeyPoints(): void {
    this.markerLayer.clearLayers();
    if (this.polyline) {
      this.polyline.remove();
      this.polyline = null;
    }

    const points = this.keyPoints();
    if (!points.length) return;

    const sorted = [...points].sort((a, b) => a.order - b.order);
    const latLngs: L.LatLngExpression[] = [];

    for (const kp of sorted) {
      const img = kp.imageUrl
        ? `<img src="${kp.imageUrl}" alt="${kp.name}" style="width:100%;max-width:200px;border-radius:4px;margin-bottom:6px;" />`
        : '';
      const marker = L.marker([kp.latitude, kp.longitude]).bindPopup(
        `${img}<strong>${kp.name}</strong><br/>${kp.description}`,
        { maxWidth: 220 }
      );
      this.markerLayer.addLayer(marker);
      latLngs.push([kp.latitude, kp.longitude]);
    }

    if (this.connectPoints() && latLngs.length > 1 && this.map) {
      this.polyline = L.polyline(latLngs, { color: '#0369a1', weight: 3 }).addTo(this.map);
    }

    if (latLngs.length && this.map) {
      this.map.fitBounds(L.latLngBounds(latLngs).pad(0.2));
    }
  }
}
