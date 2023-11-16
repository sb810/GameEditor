const cacheName = "Studio XP-Game Editor - StudioXP-2.0";
const contentToCache = [
    "Build/Build.loader.js",
    "Build/668bd8b8868948dad8be3a9cc03efae1.js.unityweb",
    "Build/fee9b258077a0d5576e16d1224ee9b45.data.unityweb",
    "Build/bb81976fa8baa1ca8e2b04d6223b5587.wasm.unityweb",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
