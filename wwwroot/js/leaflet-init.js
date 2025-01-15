export function load_map(raw) {
    try {
        // let geojson_data = [];
        let geojson_data = JSON.parse(String(raw));

        // Inisialisasi peta
        let map = L.map('map').setView({ lon: 113.9213, lat: -0.7893 }, 5);

        let indonesiaBounds = [
            [-11.0, 95.0],
            [6.0, 141.0]
        ];
        map.fitBounds(indonesiaBounds);
        map.setMaxBounds(indonesiaBounds);
        map.on('drag', function () {
            map.panInsideBounds(indonesiaBounds, { animate: true });
        });

        // Tambahkan layer tile dari OpenStreetMap
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);

        // Layer group untuk marker dan label
        let markerLayer = L.layerGroup();
        let labelLayer = L.layerGroup();
        let activeLabelMarker = null;

        // Fungsi menentukan warna berdasarkan jumlah
        function getColor(jumlah) {
            if (jumlah <= 20) return '#1A1A1A';      // Hitam
            else if (jumlah <= 40) return '#F8183E'; // Merah
            else if (jumlah <= 60) return '#FFCA0D'; // Kuning
            else if (jumlah <= 80) return '#13CE66'; // Hijau
            else return '#0D6EFD';                   // Biru
        }

        for (let geojson_item of geojson_data) {
            let lat = geojson_item.geometry.coordinates[1];
            let lon = geojson_item.geometry.coordinates[0];
            let jumlah = geojson_item.properties.jumlah;
            let branchName = geojson_item.properties.branch;

            // Buat SVG dengan warna dinamis berdasarkan `jumlah`
            let color = getColor(jumlah);
            let customIcon = L.divIcon({
                html: `<svg width="24" height="24" viewBox="0 0 24 24" fill="${color}" xmlns="http://www.w3.org/2000/svg">
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M11.262 22.134C11.262 22.134 4 16.018 4 10C4 7.87827 4.84285 5.84344 6.34315 4.34315C7.84344 2.84285 9.87827 2 12 2C14.1217 2 16.1566 2.84285 17.6569 4.34315C19.1571 5.84344 20 7.87827 20 10C20 16.018 12.738 22.134 12.738 22.134C12.334 22.506 11.669 22.502 11.262 22.134ZM12 13.5C12.4596 13.5 12.9148 13.4095 13.3394 13.2336C13.764 13.0577 14.1499 12.7999 14.4749 12.4749C14.7999 12.1499 15.0577 11.764 15.2336 11.3394C15.4095 10.9148 15.5 10.4596 15.5 10C15.5 9.54037 15.4095 9.08525 15.2336 8.66061C15.0577 8.23597 14.7999 7.85013 14.4749 7.52513C14.1499 7.20012 13.764 6.94231 13.3394 6.76642C12.9148 6.59053 12.4596 6.5 12 6.5C11.0717 6.5 10.1815 6.86875 9.52513 7.52513C8.86875 8.1815 8.5 9.07174 8.5 10C8.5 10.9283 8.86875 11.8185 9.52513 12.4749C10.1815 13.1313 11.0717 13.5 12 13.5Z" />
                    </svg>`,
                iconSize: [20, 20],
                iconAnchor: [10, 10],
                className: ""
            });

            // Tambahkan marker ke map
            let marker = L.marker([lat, lon], { icon: customIcon }).addTo(markerLayer);

            // Buat label
            let labelMarker = L.marker([lat, lon], {
                icon: L.divIcon({ className: 'my-label-icon' })
            });
            labelMarker.bindTooltip(`${branchName}: ${jumlah}`, {
                permanent: true,
                className: "my-label",
                offset: [0, 0]
            });

            // Sembunyikan label awalnya
            labelMarker.addTo(labelLayer);
            labelLayer.removeLayer(labelMarker);

            // Toggle label saat marker diklik
            marker.on('click', (e) => {
                if (activeLabelMarker) {
                    labelLayer.removeLayer(activeLabelMarker);
                }

                if (labelLayer.hasLayer(labelMarker)) {
                    labelLayer.removeLayer(labelMarker);
                    activeLabelMarker = null;
                } else {
                    labelLayer.addLayer(labelMarker);
                    activeLabelMarker = labelMarker;
                }
                e.stopPropagation();
            });
        }

        // Tambahkan layer ke map
        markerLayer.addTo(map);
        labelLayer.addTo(map);

        // Sembunyikan label aktif saat klik di area kosong peta
        map.on('click', () => {
            if (activeLabelMarker) {
                labelLayer.removeLayer(activeLabelMarker);
                activeLabelMarker = null;
            }
        });
     

        // const map = L.map('map', {
        //     center: [-1.0, 112.5],
        //     zoom: 5,
        //     zoomControl: false,
        //     scrollWheelZoom: false,
        //     doubleClickZoom: false,
        //     touchZoom: false,
        //     dragging: false
        // });
    
        // const imageUrl = '/img/map-id.svg';
        // const imageBounds = [
        //     [-10, 95],
        //     [6, 141] 
        // ];
        // const svgOverlay = L.imageOverlay(imageUrl, imageBounds).addTo(map);
        // const sitesData = [
        // {
        // name: 'Head Office',
        // coordinates: [-6.1836279026374115, 106.93109420036633],
        // jumlah: 0
        // },
        // {
        //     name: 'Medan',
        //     coordinates: [3.532271799591581, 98.73326027544451],
        //     jumlah: 0
        // },
        // {
        //     name: 'Pekanbaru',
        //     coordinates: [0.5038310050740433, 101.4191250507209],
        //     jumlah: 0
        // },
        // {
        //     name: 'Sorong',
        //     coordinates: [-0.9119621875454312, 131.32934721701946],
        //     jumlah: 0
        // },
        // {
        //     name: 'Banjarmasin',
        //     coordinates: [-3.400597213350093, 114.66638307171584],
        //     jumlah: 0
        // },
        // {
        //     name: 'Lampung',
        //     coordinates: [-5.376029380187319, 105.2452027568926],
        //     jumlah: 0
        // },
        // {
        //     name: 'Balikpapan',
        //     coordinates: [-1.2391330118140365, 116.94769870608495],
        //     jumlah: 0
        // },
        // {
        //     name: 'Jambi',
        //     coordinates: [-1.6225403474341944, 103.55099491170714],
        //     jumlah: 0
        // },
        // {
        //     name: 'Jayapura',
        //     coordinates: [-2.560040627193474, 140.70353737586936],
        //     jumlah: 0
        // },
        // {
        //     name: 'Manado',
        //     coordinates: [1.448885542037169, 124.83871319984682],
        //     jumlah: 0
        // },
        // {
        //     name: 'Padang',
        //     coordinates: [-0.9694335463210537, 100.39806110676146],
        //     jumlah: 0
        // },
        // {
        //     name: 'Palembang',
        //     coordinates: [-2.9338602262277194, 104.71754979384434],
        //     jumlah: 0
        // },
        // {
        //     name: 'Palu',
        //     coordinates: [-0.8975516786992691, 119.88940139504257],
        //     jumlah: 0
        // },
        // {
        //     name: 'Pontianak',
        //     coordinates: [-0.08348608965804316, 109.38948434708766],
        //     jumlah: 0
        // },
        // {
        //     name: 'Surabaya',
        //     coordinates: [-7.330792205142473, 112.75050342559612],
        //     jumlah: 0
        // },
        // {
        //     name: 'Samarinda',
        //     coordinates: [-0.5326897934177122, 117.09892130410779],
        //     jumlah: 0
        // },
        // {
        //     name: 'Semarang',
        //     coordinates: [-6.976731340382362, 110.32404660973297],
        //     jumlah: 0
        // },
        // {
        //     name: 'Sampit',
        //     coordinates: [-2.5266062879433604, 112.8778436783208],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tarakan',
        //     coordinates: [3.316855754541591, 117.57849169041809],
        //     jumlah: 0
        // },
        // {
        //     name: 'Ujung Pandang',
        //     coordinates: [-5.139713901677605, 119.44868361433198],
        //     jumlah: 0
        // },
        // {
        //     name: 'Adaro',
        //     coordinates: [-2.1774057959153468, 115.42238445767143],
        //     jumlah: 27
        // },
        // {
        //     name: 'Batu Kajang',
        //     coordinates: [-1.823893778177714, 115.90294131232112],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bendili',
        //     coordinates: [0.6225042058573077, 117.44811344109365],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bengalon',
        //     coordinates: [0.8190204776423108, 117.58512072288895],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bontang',
        //     coordinates: [0.12419846539615394, 117.36685361072419],
        //     jumlah: 0
        // },
        // {
        //     name: 'Buhut',
        //     coordinates: [-1.1461675931117594, 114.4887060196287],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tuhup',
        //     coordinates: [-0.4035013274630163, 114.8401700675922],
        //     jumlah: 0
        // },
        // {
        //     name: 'Muara Teweh',
        //     coordinates: [-0.9618580831260963, 114.89012831705892],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bhalada',
        //     coordinates: [-0.3804755001953996, 115.71728354417856],
        //     jumlah: 0
        // },
        // {
        //     name: 'Muaralawa',
        //     coordinates: [-0.3805581386337066, 115.71610455764295],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tabang',
        //     coordinates: [0.5326940783048452, 116.12084593892719],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bharinto',
        //     coordinates: [-0.840809223830484, 115.4932345223792],
        //     jumlah: 0
        // },
        // {
        //     name: 'Timika',
        //     coordinates: [-4.436498037502288, 136.86716061927723],
        //     jumlah: 0
        // },
        // {
        //     name: 'Rantau',
        //     coordinates: [-3.119408618058054, 115.08069426916771],
        //     jumlah: 0
        // },
        // {
        //     name: 'Sangatta',
        //     coordinates: [0.5510865351360836, 117.51854398073262],
        //     jumlah: 0
        // },
        // {
        //     name: 'Satui',
        //     coordinates: [-3.795091207221874, 115.38101928070569],
        //     jumlah: 0
        // },
        // {
        //     name: 'Sorowako',
        //     coordinates: [-2.562365204177203, 121.4114727612457],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tanjung Enim',
        //     coordinates: [-3.7344431802473976, 103.80082879122166],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tanjung Redeb',
        //     coordinates: [2.1601326101826914, 117.45674049565469],
        //     jumlah: 0
        // },
        // {
        //     name: 'Tenggarong',
        //     coordinates: [-0.2868257853849245, 117.11327920752439],
        //     jumlah: 0
        // },
        // {
        //     name: 'Ambon',
        //     coordinates: [-3.6978058715344067, 128.18408215313698],
        //     jumlah: 0
        // },
        // {
        //     name: 'Manokwari',
        //     coordinates: [-0.9300521982770971, 134.02652325095838],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bandung',
        //     coordinates: [-6.950383473648063, 107.61768477358855],
        //     jumlah: 0
        // },
        // {
        //     name: 'Banda Aceh',
        //     coordinates: [5.540801400091437, 95.30982020210595],
        //     jumlah: 0
        // },
        // {
        //     name: 'Ternate',
        //     coordinates: [0.7644880519642349, 127.37311730485015],
        //     jumlah: 0
        // },
        // {
        //     name: 'Bengkulu',
        //     coordinates: [-3.822634235980618, 102.30866510525941],
        //     jumlah: 0
        // },
        // {
        //     name: 'Pangkal Pinang',
        //     coordinates: [-2.0994965545338955, 106.11126029307631],
        //     jumlah: 0
        // },
        // {
        //     name: 'Pangkalan Bun',
        //     coordinates: [-2.705227279972859, 111.64484314108124],
        //     jumlah: 0
        // },
        // {
        //     name: 'Kendari',
        //     coordinates: [-4.002816420835838, 122.50965968758908],
        //     jumlah: 0
        // },
        // {
        //     name: 'Merauke',
        //     coordinates: [-8.5140596386602, 140.41028686856362],
        //     jumlah: 0
        // } 
        // ];
    
        // sitesData.forEach(site => {
        //     const customIcon = L.icon({
        //         iconUrl: '/img/location.svg',
        //         iconSize: [20, 20],
        //         shadowSize: [50, 64],
        //         iconAnchor: [22, 60],
        //         popupAnchor: [-3, -66]
        //     });
    
        //     const marker = L.marker(site.coordinates, { icon: customIcon });
        //     marker.bindPopup(`<strong>${site.name}</strong>: ${site.jumlah}`);
        //     marker.addTo(map);
        // });
    
        // svgOverlay.on('load', () => {
        //     const svg = document.querySelector('#map img');
    
        //     if (svg && svg.contentDocument) {
        //         const svgDoc = svg.contentDocument;
        //         const sites = svgDoc.querySelectorAll('.site');
        //         sites.forEach(site => {
        //             site.addEventListener('mouseover', () => {
        //                 site.classList.add('site-hover');
        //             });
        //             site.addEventListener('mouseout', () => {
        //                 site.classList.remove('site-hover');
        //             });
        //         });
        //     }
        // });

        return "";
    } catch (e) {
        console.error("Error parsing JSON: ", e);
    }
}
