const cacheName = "Studio XP-Game Editor - StudioXP-2.0";
const contentToCache = [
    "Build/Build.loader.js",
    "Build/97166811ab8598913a74cd68370a1a29.js.unityweb",
    "Build/7340041c53f1a83939da52ac963e202d.data.unityweb",
    "Build/74ff800f79ecbb3d3595be46d4bc55c1.wasm.unityweb",
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
